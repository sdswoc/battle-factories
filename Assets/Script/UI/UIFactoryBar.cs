using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFactoryBar : MonoBehaviour 
{
	public int currentBuildingItem;
	public GameObject slider;
	public Button factoryButton;
	public Text itemText;

	public void SetItem(int item)
	{
		currentBuildingItem = item;
		slider.SetActive(false);
		factoryButton.interactable = true;
		itemText.text = "Fact" + item.ToString();
	}
	public void OpenSlider()
	{
		factoryButton.interactable = false;
		slider.SetActive(true);
	}
}