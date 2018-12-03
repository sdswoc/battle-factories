using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour 
{
	public bool touchControl;
	public float mouseMovementFactor;
	public float mouseZoomFactor;
	public UnitSelector unitSelector;
	private CameraControl cameraControl;
	private new Camera camera;
	private Vector2 targetPosition;
	private float targetTouchDistance;
	private List<Touch> validFinger = new List<Touch>();
	private TouchMode touchMode = TouchMode.None;
	private bool upgradable = true;
	private float zoomAxis;
	private Vector2 moveAxis;

	private void Awake()
	{
		cameraControl = GetComponent<CameraControl>();
		camera = GetComponent<Camera>();
		if (Application.isMobilePlatform)
		{
			touchControl = true;
		}
	}
	private void Update()
	{
		if (touchControl)
		{
			TouchUpdate();
		}
		else
		{
			MouseUpdate();
		}
	}
	private void TouchUpdate()
	{
		int count = Input.touchCount;
		for (int i = 0; i < count; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				if (!OverlapTest(touch.position))
				{
					if (upgradable)
					{
						validFinger.Add(touch);
						if (validFinger.Count == 1)
						{
							touchMode = TouchMode.Single;
							unitSelector.KeyPressed(cameraControl.TransformCameraToWorld(touch.position));
						}
						else if (validFinger.Count == 2)
						{
							touchMode = TouchMode.Double;
							unitSelector.KeyCanceled();
							TriggerZoomStart((validFinger[0].position - validFinger[1].position).magnitude);
							TriggerPanStart((validFinger[0].position + validFinger[1].position) * 0.5f);
						}
						else
						{
							touchMode = TouchMode.None;
							upgradable = false;
						}
					}
				}
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				for (int x = 0;x < validFinger.Count;x++)
				{
					if (validFinger[x].fingerId == touch.fingerId)
					{
						validFinger[x] = touch;
						if (touchMode == TouchMode.Double)
						{
							ZoomUpdate((validFinger[0].position - validFinger[1].position).magnitude);
							PanUpdate((validFinger[0].position + validFinger[1].position) * 0.5f);
						}
						else if (touchMode == TouchMode.Single)
						{
							unitSelector.KeyMoved(cameraControl.TransformCameraToWorld(validFinger[0].position));
						}
						break;
					}
				}
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				for (int x = 0; x < validFinger.Count; x++)
				{
					if (validFinger[x].fingerId == touch.fingerId)
					{
						
						if (touchMode == TouchMode.Single)
						{
							touchMode = TouchMode.None;
							unitSelector.KeyReleased(cameraControl.TransformCameraToWorld(validFinger[0].position));
						}
						validFinger.RemoveAt(x);
						if (touchMode == TouchMode.Double)
						{
							validFinger.Clear();
							touchMode = TouchMode.None;
						}

						break;
					}
				}
			}
		}
		if (validFinger.Count == 0)
		{
			upgradable = true;
		}
	}
	private void MouseUpdate()
	{
		Vector2 mousePosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
			if (!OverlapTest(mousePosition))
			{
				unitSelector.KeyPressed(cameraControl.TransformCameraToWorld(mousePosition));
				targetPosition = mousePosition;
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if ((mousePosition-targetPosition).sqrMagnitude > 0.01f)
			{
				unitSelector.KeyMoved(cameraControl.TransformCameraToWorld(mousePosition));
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			unitSelector.KeyReleased(cameraControl.TransformCameraToWorld(mousePosition));
		}
		cameraControl.Zoom(Input.GetAxis("Zoom") * mouseZoomFactor*Time.deltaTime*camera.orthographicSize);
		cameraControl.Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * mouseMovementFactor*camera.orthographicSize * Time.deltaTime);
		cameraControl.UpdateControlPoints();
		cameraControl.AdjustZoom();
		cameraControl.UpdateControlPoints();
		cameraControl.AdjustPosition();
	}
	private bool OverlapTest(Vector2 position)
	{
		return false;
	}
	private void TriggerPanStart(Vector2 position)
	{
		targetPosition = cameraControl.TransformCameraToWorld(position);
	}
	private void TriggerZoomStart(float distance)
	{
		targetTouchDistance = distance;
	}
	private void PanUpdate(Vector2 position)
	{
		Vector2 newTargetPosition = cameraControl.TransformCameraToWorld(position);
		cameraControl.Translate(targetPosition - newTargetPosition);
		cameraControl.UpdateControlPoints();
		cameraControl.AdjustPosition();
	}
	private void ZoomUpdate(float distance)
	{
		cameraControl.Zoom(distance - targetTouchDistance);
		targetTouchDistance = distance;
		cameraControl.UpdateControlPoints();
		cameraControl.AdjustZoom();
	}
	private void OnDrawGizmosSelected()
	{
		if (touchMode == TouchMode.Single)
			Gizmos.color = Color.red;
		else if (touchMode == TouchMode.Double)
			Gizmos.color = Color.blue;
		for (int i = 0; i < validFinger.Count; i++)
		{

			Gizmos.DrawWireSphere(cameraControl.TransformCameraToWorld(validFinger[i].position), 0.5f);
		}
	}
}

enum TouchMode
{
	None,Single,Double
}