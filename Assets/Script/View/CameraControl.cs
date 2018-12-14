using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View
{
	public class CameraControl : MonoBehaviour
	{
		public float minZoomFactor;
		public float maxZoomFactor;

		private new Camera camera;
		private new Transform transform;
		private float width;
		private float height;
		private Vector2[] controlPoints = new Vector2[4];
		private Vector2 minPoint;
		private Vector2 maxPoint;
		private Vector2 virtualXAxis;
		private Vector2 virtualYAxis;
		private Vector2[] mapControlPoints = new Vector2[4];
		[HideInInspector]
		public Vector2 cameraResolution;

		private void Awake()
		{
			GameFlow.cameraControl = this;
			camera = GetComponent<Camera>();
			GameFlow.camera = camera;
			GameFlow.cameraTransform = GetComponent<Transform>();
			transform = GetComponent<Transform>();
			cameraResolution = new Vector2(camera.pixelWidth, camera.pixelHeight);
		}
		
		public Vector2 TransformCameraToWorld(Vector2 camCord)
		{
			width = camera.orthographicSize * camera.aspect * 2;
			height = camera.orthographicSize * 2;

			Vector3 origin = transform.position + transform.right * (-0.5f + camCord.x / cameraResolution.x) * width + transform.up * (-0.5f + camCord.y / cameraResolution.y) * height;
			Vector3 direction = transform.forward;
			float factor = -origin.z / direction.z;
			return (Vector2)(origin + direction * factor);
		}

		public bool AdjustZoom()
		{
			UpdateControlPoints();
			bool adjusted = false;
			float spanWidth, spanHeight, constrainWidth, constrainHeight;
			spanWidth = Vector2.Dot((controlPoints[3] - controlPoints[2]),virtualXAxis);
			spanHeight = Vector2.Dot(controlPoints[1] - controlPoints[2],virtualYAxis);
			constrainWidth = (maxPoint-minPoint).x;
			constrainHeight = (maxPoint-minPoint).y;
			if (spanWidth / spanHeight > constrainWidth / constrainHeight)
			{
				if (spanWidth > constrainWidth)
				{
					camera.orthographicSize *= constrainWidth / spanWidth;
					adjusted = true;
				}
			}
			else
			{
				if (spanHeight > constrainHeight)
				{
					camera.orthographicSize *= constrainHeight / spanHeight;
					adjusted = true;
				}
			}
			if (camera.orthographicSize < minZoomFactor || camera.orthographicSize > maxZoomFactor)
			{
				adjusted = true;
				camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoomFactor, maxZoomFactor);
			}
			return adjusted;
		}
		
		public bool AdjustPosition()
		{
			UpdateControlPoints();
			bool adjusted = false;
			Vector2 min = virtualXAxis * minPoint.x + virtualYAxis * minPoint.y;
			Vector2 max = virtualXAxis * maxPoint.x + virtualYAxis * maxPoint.y;
			Debug.DrawLine(min, max);
			Debug.DrawLine(min, controlPoints[2], Color.gray);
			min = controlPoints[2] - min;
			max = controlPoints[0] - max;

			Vector2 displacement = Vector2.zero;
			if (Vector2.Dot(min,virtualXAxis) < 0)
			{
				displacement.x -= Vector2.Dot(min, virtualXAxis);
				adjusted = true;
			}
			if (Vector2.Dot(min, virtualYAxis) < 0)
			{
				displacement.y -= Vector2.Dot(min, virtualYAxis);
				adjusted = true;
			}
			if (Vector2.Dot(max, virtualXAxis) > 0)
			{
				displacement.x -= Vector2.Dot(max, virtualXAxis);
				adjusted = true;
			}
			if (Vector2.Dot(max, virtualYAxis) > 0)
			{
				displacement.y -= Vector2.Dot(max, virtualYAxis);
				adjusted = true;
			}
			transform.position += (Vector3)(displacement.x*virtualXAxis+displacement.y*virtualYAxis);
			return adjusted;
		}
		
		public void Translate(Vector2 displacement)
		{
			transform.position += (Vector3)displacement;
		}
		
		public void Zoom(float size)
		{
			camera.orthographicSize = Mathf.Clamp(camera.orthographicSize * (1 - size / ((cameraResolution.x + cameraResolution.y) * 0.5f)), minZoomFactor, maxZoomFactor);
		}

		public void Focus(Vector2 focus)
		{
			Vector2 current = TransformCameraToWorld(new Vector2(cameraResolution.x, cameraResolution.y) * 0.5f);
			camera.GetComponent<Transform>().Translate(focus - current);
		}
		
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			for (int i = 0; i < controlPoints.Length; i++)
			{
				Gizmos.DrawLine(controlPoints[(i + 1) % controlPoints.Length], controlPoints[i]);
			}
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(new Vector3(minPoint.x, minPoint.y), new Vector3(minPoint.x, maxPoint.y));
			Gizmos.DrawLine(new Vector3(minPoint.x, minPoint.y), new Vector3(maxPoint.x, minPoint.y));
			Gizmos.DrawLine(new Vector3(minPoint.x, maxPoint.y), new Vector3(maxPoint.x, maxPoint.y));
			Gizmos.DrawLine(new Vector3(maxPoint.x, minPoint.y), new Vector3(maxPoint.x, maxPoint.y));
		}
		
		public void UpdateControlPoints()
		{
			width = camera.orthographicSize * camera.aspect * 2;
			height = camera.orthographicSize * 2;
			controlPoints[0] = TransformCameraToWorld(new Vector2(cameraResolution.x, cameraResolution.y));
			controlPoints[1] = TransformCameraToWorld(new Vector2(0, cameraResolution.y));
			controlPoints[2] = TransformCameraToWorld(new Vector2(0, 0));
			controlPoints[3] = TransformCameraToWorld(new Vector2(cameraResolution.x, 0));

			virtualXAxis = (controlPoints[0] - controlPoints[1]).normalized;
			virtualYAxis = (controlPoints[1] - controlPoints[2]).normalized;
			mapControlPoints[0] = GameFlow.map.GetCameraMinimum();
			mapControlPoints[1] = new Vector2(GameFlow.map.GetCameraMaximum().x, GameFlow.map.GetCameraMinimum().y);
			mapControlPoints[2] = GameFlow.map.GetCameraMaximum();
			mapControlPoints[3] = new Vector2(GameFlow.map.GetCameraMinimum().x, GameFlow.map.GetCameraMaximum().y);

			minPoint = maxPoint = new Vector2(Vector2.Dot(mapControlPoints[0], virtualXAxis), Vector2.Dot(mapControlPoints[0], virtualYAxis));
			for (int i = 1; i < 4; i++)
			{
				if (Vector2.Dot(mapControlPoints[i], virtualXAxis) < minPoint.x)
				{
					minPoint.x = Vector2.Dot(mapControlPoints[i], virtualXAxis);
				}
				else if (Vector2.Dot(mapControlPoints[i], virtualXAxis) > maxPoint.x)
				{
					maxPoint.x = Vector2.Dot(mapControlPoints[i], virtualXAxis);
				}
				if (Vector2.Dot(mapControlPoints[i], virtualYAxis) < minPoint.y)
				{
					minPoint.y = Vector2.Dot(mapControlPoints[i], virtualYAxis);
				}
				else if (Vector2.Dot(mapControlPoints[i], virtualYAxis) > maxPoint.y)
				{
					maxPoint.y = Vector2.Dot(mapControlPoints[i], virtualYAxis);
				}
			}
		}
	}
}