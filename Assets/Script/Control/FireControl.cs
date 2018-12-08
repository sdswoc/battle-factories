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
		yield return new WaitForSeconds(3);
		EventHandle.FinishFire(myTurn);
	}

	public void EvaluateHP(Vector2Int[] hps,bool myTurn)
	{
		FinishEvaluation(myTurn);
	}
	public void EvaluateHP(bool myTurn)
	{
		FinishEvaluation(myTurn);
	}
	public void FinishEvaluation(bool myTurn)
	{
		EventHandle.SetTurn(!myTurn);
	}
}
public struct FireEvent
{
	Unit dealer;
	Unit target;
}