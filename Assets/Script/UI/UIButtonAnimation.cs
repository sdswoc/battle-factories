using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonAnimation : MonoBehaviour 
{
	public Button button;
	/*
	public RectTransform background;
	public RectTransform foreground;
	public Image[] sampleImages;
	public Image[] sampleTexts;
	public Color enabledColor;
	public Color disabledColor;
	*/
	private bool enable = true;
	private bool pressed = false;

	private void Update()
	{
		button.interactable = button.interactable;
	}
}
