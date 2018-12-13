using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour 
{
	public List<FireEvent> fireEvents = new List<FireEvent>();

	private void Awake()
	{
		GameFlow.fireControl = this;
	}
	private void Start()
	{
		//StartCoroutine(TempFire(false));
		//FinishEvaluation(false);
	}
	public void Execute(bool myTurn)
	{
		StopAllCoroutines();
		StartCoroutine(TempFire(myTurn));
	}

	IEnumerator TempFire(bool myTurn)
	{
		GameFlow.fireIndicator.StartFire();
		for (int i = 0; i < GameFlow.units.Count; i++)
		{
			Debug.Log("Rename");
			Troop t = null;
			t = GameFlow.units[i] as Troop;
			if (t != null)
			{
				t.StopAllCoroutines();
				t.EndPath(t.position);
			}
		}
		yield return new WaitForEndOfFrame();
		for (int i = 0;i < GameFlow.units.Count;i++)
		{
			Troop t = null;
			t = GameFlow.units[i] as Troop;
			t?.GenerateAttackList();
			if (t != null)
			{
				yield return StartCoroutine(t.Attack());
			}
		}
		yield return new WaitForEndOfFrame();
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
		yield return new WaitUntil(() => Projectile.list.Count <= 0);
		for (int i = 0;i < GameFlow.units.Count;i++)
		{
			if (GameFlow.units[i].hp <= 0)
			{
				GameFlow.units[i].Despawn();
				i--;
			}
		}
		GameFlow.fireIndicator.CloseFire();
		EventHandle.SetTurn(!myTurn);
	}
}
public struct FireEvent
{
	Troop dealer;
	Troop target;
}