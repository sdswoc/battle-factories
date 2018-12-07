using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
namespace UI
{
	public class UISetupMode : MonoBehaviour
	{
		public void OnOKButton()
		{
			GameFlow.setupFactory.OnOKButton();
		}
	}
}