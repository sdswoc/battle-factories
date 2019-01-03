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
using HUD;
using GameElements;
using Army;
using Weapon;

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
    public static Flag flag;
    public static MenuScript menuScript;
	public static int money;
	public static int fuel;
	public static int moneyRate = 100;
	public static int fuelLimit = 5;
    public static int flagCapture = 1;
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
    public static void GenerateCircleMesh(Mesh m, float radius1, float radius2, int detail)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < detail; i++)
        {
            vertices.Add(new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)detail) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)detail) * 2)) * radius1);
            vertices.Add(new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)detail) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)detail) * 2)) * (radius2));
            triangles.Add((i * 2 + 0) % (detail * 2));
            triangles.Add((i * 2 + 1) % (detail * 2));
            triangles.Add((i * 2 + 2) % (detail * 2));
            triangles.Add((i * 2 + 1) % (detail * 2));
            triangles.Add((i * 2 + 3) % (detail * 2));
            triangles.Add((i * 2 + 2) % (detail * 2));
        }
        m.SetVertices(vertices);
        m.SetTriangles(triangles, 0);
        m.RecalculateNormals();
        m.RecalculateBounds();
    }
    public static void SetMeshColor(Mesh m, Color c)
    {
        List<Color> colorList = new List<Color>();
        for (int i = 0; i < m.vertexCount; i++)
        {
            colorList.Add(c);
        }
        m.SetColors(colorList);
    }
}
