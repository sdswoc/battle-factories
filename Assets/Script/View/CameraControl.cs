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
		private Vector2 cameraResolution;

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
			bool adjusted = false;
			/*
				
				float spanWidth, spanHeight, constrainWidth, constrainHeight;
				spanWidth = maxPoint.x - minPoint.x;
				spanHeight = maxPoint.y - minPoint.y;
				constrainWidth = GameFlow.map.GetCameraWidth();
				constrainHeight = GameFlow.map.GetCameraHeight();
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
				*/
			if (camera.orthographicSize < minZoomFactor || camera.orthographicSize > maxZoomFactor)
				{
					adjusted = true;
					camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoomFactor, maxZoomFactor);
				}
				return adjusted;
		}
		
		public bool AdjustPosition()
		{/*
			bool adjusted = false;
			Vector2 displacement = Vector2.zero;
			if (minPoint.x < 0)
			{
				displacement.x -= minPoint.x;
				adjusted = true;
			}
			if (minPoint.y < 0)
			{
				displacement.y -= minPoint.y;
				adjusted = true;
			}
			if (maxPoint.x > GameFlow.map.GetCameraWidth())
			{
				displacement.x -= maxPoint.x - GameFlow.map.GetCameraWidth();
				adjusted = true;
			}
			if (maxPoint.y > GameFlow.map.GetCameraHeight())
			{
				displacement.y -= maxPoint.y - GameFlow.map.GetCameraHeight();
				adjusted = true;
			}
			transform.position += (Vector3)displacement;
			return adjusted;
			*/
			return false;
		}
		
		public void Translate(Vector2 displacement)
		{
			transform.position += (Vector3)displacement;
		}
		
		public void Zoom(float size)
		{
			camera.orthographicSize = Mathf.Clamp(camera.orthographicSize * (1 - size / ((cameraResolution.x + cameraResolution.y) * 0.5f)), minZoomFactor, maxZoomFactor);
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

			minPoint = controlPoints[0];
			maxPoint = controlPoints[0];
			for (int i = 0; i < controlPoints.Length; i++)
			{
				if (controlPoints[i].x < minPoint.x)
				{
					minPoint.x = controlPoints[i].x;
				}
				if (controlPoints[i].y < minPoint.y)
				{
					minPoint.y = controlPoints[i].y;
				}
				if (controlPoints[i].x > maxPoint.x)
				{
					maxPoint.x = controlPoints[i].x;
				}
				if (controlPoints[i].y > maxPoint.y)
				{
					maxPoint.y = controlPoints[i].y;
				}
			}
		}
	}
}