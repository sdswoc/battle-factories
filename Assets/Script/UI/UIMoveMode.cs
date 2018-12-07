using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class UIMoveMode : MonoBehaviour
	{
		public void OnSpawnButton(int i)
		{
			EventHandle.CreateFriendlyUnit((byte)i);
		}
	}
}