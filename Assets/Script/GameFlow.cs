using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Control;
using View;
using UI;
using Grid;
public class GameFlow 
{
	public static UnitSelector unitSelector;
	public static CameraControl cameraControl;
	public static CameraInput cameraInput;
	public static ControlRelay controlRelay;
	public static SetupFactory setupFactory;
	public static Map map;
	public static UIPanelSwitcher uiPanelSwitcher;
	public static EventSystem eventSystem;
	public static GraphicRaycaster graphicRaycaster;
}
