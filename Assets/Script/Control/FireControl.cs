using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireControl : MonoBehaviour 
{
    public string firePopUpText;
    public AudioSource deathSoundSource;
    public AudioSource bgMusic;
	private void Awake()
	{
		GameFlow.fireControl = this;
	}
	public void Execute(bool myTurn)
	{
		StopAllCoroutines();
		StartCoroutine(TempFire(myTurn));
        GameFlow.uiTutorialText.Pop(firePopUpText);
	}
	IEnumerator TempFire(bool myTurn)
	{
		GameFlow.fireIndicator.StartFire();
        bool fired = false;
        bgMusic.pitch = 1.25f;
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
                    fired = true;
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
        for (int i = 0;i < GameFlow.units.Count;i++)
		{
			GameFlow.units[i].hp = Mathf.Clamp(GameFlow.units[i].finalHp,0,GameFlow.units[i].maxHp);
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
		bool friendly = GameFlow.friendlyFactory.GetComponent<Unit>().hp <= 0;
		bool enemy = GameFlow.enemyFactory.GetComponent<Unit>().hp <= 0;
		if (friendly)
		{
			Factory f = GameFlow.friendlyFactory;
			GameFlow.cameraInput.active = false;
			GameFlow.cameraControl.Focus((Vector2)f.position);
			f.cloudTrigger.Hide();
			yield return new WaitForSeconds(0.3f);
			for (float i = 0;i < 1;i += Time.deltaTime)
			{
				f.GetComponent<Transform>().position = (Vector3)(Vector2)f.position + Vector3.forward * i * 2;
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(0.8f);
		}

		if (enemy)
		{
			Factory f = GameFlow.enemyFactory;
			GameFlow.cameraInput.active = false;
			GameFlow.cameraControl.Focus((Vector2)f.position);
			f.cloudTrigger.Hide();
			yield return new WaitForSeconds(0.3f);
			for (float i = 0; i < 1; i += Time.deltaTime)
			{
				f.GetComponent<Transform>().position = (Vector3)(Vector2)f.position + Vector3.forward * i * 2;
				yield return new WaitForEndOfFrame();
			}
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
public struct FireEvent
{
	Troop dealer;
	Troop target;
}