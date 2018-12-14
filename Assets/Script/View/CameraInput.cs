using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Control;

namespace View
{
	public class CameraInput : MonoBehaviour
	{
		public bool active = true;
		public bool touchControl;
		public float mouseMovementFactor;
		public float mouseZoomFactor;
		public float touchThreshold;

		private PointerEventData pointerEventData;
		private CameraControl cameraControl;
		private new Camera camera;
		private Vector2 targetPosition;
		private float targetTouchDistance;
		private List<Touch> validFinger = new List<Touch>();
		private TouchMode touchMode = TouchMode.None;
		private bool upgradable = true;
		private Vector2 moveAxis;
		private float cameraZoomVelocity;
		private float cameraZoomTargetSize;
		private float cameraZoomCurrentSize;

		private void Awake()
		{
			GameFlow.cameraInput = this;
			cameraControl = GetComponent<CameraControl>();
			camera = GetComponent<Camera>();
			if (Application.isMobilePlatform)
			{
				touchControl = true;
			}
			cameraZoomVelocity = 0;
			cameraZoomTargetSize = cameraZoomCurrentSize = camera.orthographicSize;
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
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GameFlow.SetMode((UIMode)(((int)GameFlow.uiMode+1)%2));
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
								GameFlow.controlRelay.KeyPressed(cameraControl.TransformCameraToWorld(touch.position));
							}
							else if (validFinger.Count == 2)
							{
								touchMode = TouchMode.Double;
								GameFlow.controlRelay.KeyCanceled();
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
					for (int x = 0; x < validFinger.Count; x++)
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
								GameFlow.controlRelay.KeyMoved(cameraControl.TransformCameraToWorld(validFinger[0].position));
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
								GameFlow.controlRelay.KeyReleased(cameraControl.TransformCameraToWorld(validFinger[0].position));
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
			bool lmb = Input.GetMouseButton(0);
			bool rmb = Input.GetMouseButton(1);
			int scroll = -Mathf.RoundToInt(Input.mouseScrollDelta.y);
			if (Input.GetMouseButtonDown(0) && !rmb)
			{
				if (!OverlapTest(mousePosition))
				{
					GameFlow.controlRelay.KeyPressed(cameraControl.TransformCameraToWorld(mousePosition));
					targetPosition = mousePosition;
					upgradable = false;
				}
			}
			else if (lmb)
			{
				if (!upgradable)
				{
					if ((mousePosition - targetPosition).sqrMagnitude > 0.01f)
					{
						GameFlow.controlRelay.KeyMoved(cameraControl.TransformCameraToWorld(mousePosition));
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (!upgradable)
				{
					GameFlow.controlRelay.KeyReleased(cameraControl.TransformCameraToWorld(mousePosition));
				}
				upgradable = true;
			}
			if (Input.GetMouseButtonDown(1))
			{
				TriggerPanStart(mousePosition);
			}
			else if (rmb)
			{
				PanUpdate(mousePosition);
			}
			if (Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				cameraZoomTargetSize += mouseZoomFactor * camera.orthographicSize;
			}
			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				cameraZoomTargetSize -= mouseZoomFactor * camera.orthographicSize;
			}
			cameraZoomTargetSize += scroll * mouseZoomFactor*camera.orthographicSize;
			cameraZoomCurrentSize = Mathf.SmoothDamp(cameraZoomCurrentSize, cameraZoomTargetSize, ref cameraZoomVelocity, 0.2f);
			camera.orthographicSize = (cameraZoomCurrentSize);
			if (!rmb)
			{
				cameraControl.Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * mouseMovementFactor * camera.orthographicSize * Time.deltaTime);
			}
			cameraControl.UpdateControlPoints();
			if (cameraControl.AdjustZoom())
			{
				cameraZoomTargetSize = cameraZoomCurrentSize = camera.orthographicSize;
			}
			cameraControl.UpdateControlPoints();
			cameraControl.AdjustPosition();
		}

		private bool OverlapTest(Vector2 position)
		{
			pointerEventData = new PointerEventData(GameFlow.eventSystem);
			pointerEventData.position = position;
			List<RaycastResult> result = new List<RaycastResult>();
			GameFlow.graphicRaycaster.Raycast(pointerEventData, result);
			return (result.Count > 0);
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
			if (active)
			{
				Vector2 newTargetPosition = cameraControl.TransformCameraToWorld(position);
				cameraControl.Translate(targetPosition - newTargetPosition);
				cameraControl.UpdateControlPoints();
				cameraControl.AdjustPosition();
			}
		}

		private void ZoomUpdate(float distance)
		{
			cameraControl.Zoom(distance - targetTouchDistance);
			targetTouchDistance = distance;
			cameraControl.UpdateControlPoints();
			cameraControl.AdjustZoom();
		}

		private void OnDrawGizmos()
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

		enum TouchMode
		{
			None, Single, Double
		}
	}
}