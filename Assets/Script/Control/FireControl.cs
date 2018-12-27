using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public void Execute(bool myTurn)
	{
		StopAllCoroutines();
		StartCoroutine(TempFire(myTurn));
	}
	IEnumerator TempFire(bool myTurn)
	{
		GameFlow.fireIndicator.StartFire();
        bool fired = false;

		for (int i = 0; i < GameFlow.units.Count; i++)
		{
			Troop t = null;
			t = GameFlow.units[i] as Troop;
			if (t != null)
			{
				t.StopAllCoroutines();
				t.EndPath(t.position);
			}
		}
		for (int i = 0;i < GameFlow.units.Count;i++)
		{
			Troop t = null;
			t = GameFlow.units[i] as Troop;
			t?.GenerateAttackList();
            if (t != null)
            {
                if (t.attackList.Count > 0)
                {
                    if (!fired)
                    {
                        fired = true;
                        GameFlow.uiTutorialText.Pop(firePopUpText);
                        bgMusic.pitch = 1.25f;
                    }
                }
                yield return StartCoroutine(t.Attack());
            }
		}
        bgMusic.pitch = 0.95f;
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
	public void EvaluateHP(Vector2Int[] hps,bool myTurn)
	{
		for (int i = 0;i < hps.Length;i++)
		{
			int id = hps[i].x;
			for (int j = 0;j < GameFlow.units.Count;j++)
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
        Debug.Log("Waiting");
        yield return new WaitUntil(() => GameFlow.projectiles.Count <= 0);
        bool first = false;
        for (int i = 0; i < GameFlow.units.Count; i++)
        {
            GameFlow.units[i].hp = Mathf.Clamp(GameFlow.units[i].finalHp, 0, GameFlow.units[i].maxHp);
            GameFlow.units[i].finalHp = GameFlow.units[i].hp;
            GameFlow.units[i].hpIndicator.UpdateMesh();
            if (GameFlow.units[i] as Troop != null)
                (GameFlow.units[i] as Troop).selectable = true;
            if (GameFlow.units[i].hp <= 0)
            {
                if (!first)
                {
                    first = true;
                    yield return new WaitForSeconds(1);
                    deathSoundSource.Play();
                }
                GameFlow.units[i].Despawn();

                i--;
            }
        }
        yield return new WaitForEndOfFrame();
        int friendlyCapturing = 0;
        int enemyCapturing = 0;
        for (int i = 0; i < GameFlow.units.Count; i++)
        {
            Unit u = GameFlow.units[i];
            if ((u.position - GameFlow.flag.position).sqrMagnitude < ValueLoader.flagRange * ValueLoader.flagRange)
            {
                if (u.type == UnitType.Friendly)
                {
                    friendlyCapturing++;
                }
                else
                {
                    enemyCapturing++;
                }
            }
        }
        bool focus = false;
        if (friendlyCapturing != 0 && enemyCapturing == 0)
        {
            GameFlow.flagCapture++;
            focus = true;
            trumpetSound.Play();
        }
        else if (friendlyCapturing == 0 && enemyCapturing != 0)
        {
            GameFlow.flagCapture--;
            focus = true;
            trumpetSound.Play();
        }
        if (focus)
        {
            GameFlow.cameraInput.active = false;
            Vector2 cameraPosition = GameFlow.cameraControl.TransformCameraToWorld(GameFlow.cameraControl.cameraResolution * 0.5f);
            yield return StartCoroutine(GameFlow.cameraControl.Focus(GameFlow.flag.position, 0.2f));
            yield return new WaitForSeconds(2.6f);
            if (Mathf.Abs(GameFlow.flagCapture) < ValueLoader.flagCaptureTurnLimit)
            {
                yield return StartCoroutine(GameFlow.cameraControl.Focus(cameraPosition, 0.2f));
            }
            GameFlow.cameraInput.active = true;
        }
        bool friendly = GameFlow.friendlyFactory.GetComponent<Unit>().hp <= 0;
        bool enemy = GameFlow.enemyFactory.GetComponent<Unit>().hp <= 0;
        bool flag = false;
        if (Mathf.Abs(GameFlow.flagCapture) >= ValueLoader.flagCaptureTurnLimit)
        {
            flag = true;
        }
        if (flag)
        {
            GameFlow.cameraInput.active = false;
            yield return StartCoroutine(GameFlow.cameraControl.Focus((Vector2)GameFlow.flag.position,0.3f));
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
        else
        {
            if (friendly)
            {
                Factory f = GameFlow.friendlyFactory;
                GameFlow.cameraInput.active = false;
                yield return StartCoroutine(GameFlow.cameraControl.Focus((Vector2)f.position,0.3f));
                f.cloudTrigger.Hide();
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(f.Despawn());
                yield return new WaitForSeconds(0.8f);
            }

            if (enemy)
            {
                Factory f = GameFlow.enemyFactory;
                GameFlow.cameraInput.active = false;
                yield return StartCoroutine(GameFlow.cameraControl.Focus((Vector2)f.position,0.3f));
                f.cloudTrigger.Hide();
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(f.Despawn());
                yield return new WaitForSeconds(0.8f);
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
}