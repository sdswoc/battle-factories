using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameElements;
using Army;
using Singleton;

namespace Control
{
    public class FireControl : MonoBehaviour
    {
        public string firePopUpText;
        public AudioSource deathSoundSource;
        public AudioSource bgMusic;
        public AudioSource trumpetSound;
        private void Awake()
        {
            GameFlow.fireControl = this;
        }
        public void ExecuteFiring(bool myTurn)
        {
            StopAllCoroutines();
            StartCoroutine(FireCoroutine(myTurn));
        }
        IEnumerator FireCoroutine(bool myTurn)
        {
            bool fired = false;
            ResetUnits();
            for (int i = 0; i < GameFlow.units.Count; i++)
            {
                Troop t = GameFlow.units[i] as Troop;
                t?.GenerateAttackList();
                if (t != null && t.attackList.Count > 0)
                {
                    if (!fired)
                    {
                        fired = true;
                        GameFlow.uiTutorialText.Pop(firePopUpText);
                        GameFlow.fireIndicator.StartFire();
                        bgMusic.pitch = 1.25f;
                    }
                    yield return StartCoroutine(t.Attack());
                }
            }
            if (fired)
            {
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            EventHandle.FinishFire(myTurn);
        }
        public void EvaluateHP(Vector2Int[] hps, bool myTurn)
        {
            for (int i = 0; i < hps.Length; i++)
            {
                int id = hps[i].x;
                for (int j = 0; j < GameFlow.units.Count; j++)
                {
                    if (GameFlow.units[j].unitID == id)
                    {
                        GameFlow.units[j].hp = hps[i].y;
                        break;
                    }
                }
            }
            StartCoroutine(FinishEvaluation(myTurn));
        }
        public void EvaluateHP(bool myTurn)
        {
            StartCoroutine(FinishEvaluation(myTurn));
        }
        public IEnumerator FinishEvaluation(bool myTurn)
        {
            yield return new WaitUntil(() => GameFlow.projectiles.Count <= 0);
            if (DespawnUnits())
            {
                yield return new WaitForSeconds(0.6f);
                deathSoundSource.Play();
            }
            if (CalculateCapture())
            {
                yield return StartCoroutine(FocusFlagCoroutine());
            }
            if (Mathf.Abs(GameFlow.flagCapture) >= ValueLoader.flagCaptureTurnLimit)
            {
                StartCoroutine(FlagFinishCoroutine());
            }
            else
            {
                bool friendly = GameFlow.friendlyFactory.GetComponent<Unit>().hp <= 0;
                bool enemy = GameFlow.enemyFactory.GetComponent<Unit>().hp <= 0;
                if (friendly)
                {
                    yield return StartCoroutine(FocusFactoryCoroutine(GameFlow.friendlyFactory));
                }
                if (enemy)
                {
                    yield return StartCoroutine(FocusFactoryCoroutine(GameFlow.enemyFactory));
                }
                if (friendly || enemy)
                {
                    yield return new WaitForSeconds(0.8f);
                }
                if (friendly && !enemy)
                {
                    yield return StartCoroutine(ShowFinishMessage(1));
                }
                else if (!friendly && enemy)
                {
                    yield return StartCoroutine(ShowFinishMessage(0));
                }
                else if (friendly && enemy)
                {
                    yield return StartCoroutine(ShowFinishMessage(2));
                }
            }
            GameFlow.fireIndicator.CloseFire();
            EventHandle.SetTurn(!myTurn);
        }
        public IEnumerator ShowFinishMessage(int code)
        {
            yield return new WaitForEndOfFrame();
            GameFlow.timer.CloseTime();
            GameFlow.fireIndicator.CloseFire();
            GameFlow.cameraInput.active = false;
            GameFlow.finishPanel.gameObject.SetActive(true);
            yield return StartCoroutine(GameFlow.finishPanel.Show(code));
            GameFlow.uiCurtain.gameObject.SetActive(true);
            yield return StartCoroutine(GameFlow.uiCurtain.Close());
            EventHandle.GoToMainMenu();
        }
        private bool DespawnUnits()
        {
            bool returnValue = false;
            for (int i = 0; i < GameFlow.units.Count; i++)
            {
                GameFlow.units[i].hp = Mathf.Clamp(GameFlow.units[i].finalHp, 0, GameFlow.units[i].maxHp);
                GameFlow.units[i].finalHp = GameFlow.units[i].hp;
                GameFlow.units[i].hpIndicator.UpdateMesh();
                if (GameFlow.units[i] as Troop != null)
                    (GameFlow.units[i] as Troop).selectable = true;
                if (GameFlow.units[i].hp <= 0)
                {
                    returnValue = true;
                    GameFlow.units[i].Despawn();
                    i--;
                }
            }
            return returnValue;
        }
        private bool CalculateCapture()
        {
            bool friendlyCapturing = false;
            bool enemyCapturing = false;
            for (int i = 0; i < GameFlow.units.Count; i++)
            {
                Unit u = GameFlow.units[i];
                if ((u.position - GameFlow.flag.position).sqrMagnitude < ValueLoader.flagRange * ValueLoader.flagRange)
                {
                    friendlyCapturing = (u.type == UnitType.Friendly) ? true : friendlyCapturing;
                    enemyCapturing = (u.type == UnitType.Enemy) ? true : enemyCapturing;
                }
            }
            if (friendlyCapturing && !enemyCapturing)
            {
                GameFlow.flagCapture++;
                trumpetSound.Play();
                return true;
            }
            else if (!friendlyCapturing&& enemyCapturing)
            {
                GameFlow.flagCapture--;
                trumpetSound.Play();
                return true;
            }
            return false;
        }
        private IEnumerator FocusFlagCoroutine()
        {
            GameFlow.cameraInput.active = false;
            Vector2 cameraPosition = GameFlow.cameraControl.TransformCameraToWorld(GameFlow.cameraControl.cameraResolution * 0.5f);
            yield return StartCoroutine(GameFlow.cameraControl.Focus(GameFlow.flag.position, 0.2f));
            yield return new WaitForSeconds(2.4f);
            if (Mathf.Abs(GameFlow.flagCapture) < ValueLoader.flagCaptureTurnLimit)
            {
                yield return StartCoroutine(GameFlow.cameraControl.Focus(cameraPosition, 0.2f));
            }
            GameFlow.cameraInput.active = true;
        }
        private IEnumerator FocusFactoryCoroutine(Factory factory)
        {
            GameFlow.cameraInput.active = false;
            yield return StartCoroutine(GameFlow.cameraControl.Focus((Vector2)factory.position, 0.3f));
            factory.cloudTrigger.Hide();
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(factory.DespawnCoroutine());
            yield return new WaitForSeconds(0.8f);
        }
        private IEnumerator FlagFinishCoroutine()
        {
            GameFlow.cameraInput.active = false;
            yield return StartCoroutine(GameFlow.cameraControl.Focus((Vector2)GameFlow.flag.position, 0.3f));
            GameFlow.flag.Play();
            yield return new WaitForSeconds(1.3f);
            if (GameFlow.flagCapture < 0)
            {
                yield return StartCoroutine(ShowFinishMessage(1));
            }
            else
            {
                yield return StartCoroutine(ShowFinishMessage(0));
            }
        }
        private void ResetUnits()
        {
            for (int i = 0; i < GameFlow.units.Count; i++)
            {
                Troop t = GameFlow.units[i] as Troop;
                t?.EndPath(t.position);
            }
        }
    }
}