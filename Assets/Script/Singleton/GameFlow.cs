using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Control;
using View;
using UI;
using Grid;
using Pathfinding;
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
	public static AStar aStar;
	public static EventHandle eventHandle;
	public static Factory friendlyFactory;
	public static Factory enemyFactory;
	public static UIMode uiMode;
	public static PotentialMap potentialMap;
	public static UIWaitMode uiWaitMode;
	public static UIMoveMode uiMoveMode;
	public static UISetupMode uiSetupMode;
	public static UIFireMode uiFireMode;
	public static FireControl fireControl;
	public static Transform cameraTransform;
	public static Camera camera;
	public static BillboardManager billboardManager;
	public static UISpawnModule uiSpawnUnit;
	public static UIResourceCounter uIResourceCounter;
	public static List<Unit> units = new List<Unit>();
	public static List<Projectile> projectiles = new List<Projectile>();
	public static Timer timer;
	public static FireIndicator fireIndicator;
	public static UIFinishPanel finishPanel;
	public static UIDisconnectPanel disconnectPanel;
	public static UICurtain uiCurtain;
    public static UITutorialText uiTutorialText;
	public static int money;
	public static int fuel;
	public static int moneyRate = 100;
	public static int fuelLimit = 5;
	public static string friendlyName = "Unnamed Player";
	public static string enemyName = "Unnamed Player";

	public static float FACTORY_SETUP_TIMELIMIT = 10;
	public static float TURN_TIME_LIMIT = 20;


	public static void SetMode(UIMode mode)
	{
		uiPanelSwitcher.SwitchUIMode(mode);
		controlRelay.SwitchUIMode(mode);
		uiMode = mode;
	}
}
