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
	public void Execute(bool myTurn)
	{
		Debug.Log("Execute" + myTurn.ToString());
		StopAllCoroutines();
		StartCoroutine(TempFire(myTurn));
	}

	IEnumerator TempFire(bool myTurn)
	{
		Debug.Log("Temp fire");
		for (int i = 0;i < Unit.units.Count;i++)
		{
			Unit.units[i].GenerateAttackList();
			yield return StartCoroutine(Unit.units[i].Attack());
		}
		EventHandle.FinishFire(myTurn);
	}

	public void EvaluateHP(Vector2Int[] hps,bool myTurn)
	{
		for (int i = 0;i < hps.Length;i++)
		{
			int id = hps[i].x;
			for (int j = 0;j < Unit.units.Count;j++)
			{
				if (Unit.units[j].unitID == id)
				{
					Unit.units[j].hp = hps[i].y;
					break;
				}
			}
		}
		FinishEvaluation(myTurn);
	}
	public void EvaluateHP(bool myTurn)
	{
		
		FinishEvaluation(myTurn);
	}
	public void FinishEvaluation(bool myTurn)
	{
		for (int i = 0;i < Unit.units.Count;i++)
		{
			if (Unit.units[i].hp <= 0)
			{
				Unit.units[i].Despawn();
			}
		}
		EventHandle.SetTurn(!myTurn);
	}
}
public struct FireEvent
{
	Unit dealer;
	Unit target;
}