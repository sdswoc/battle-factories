using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
using UnityEngine.UI;
using Multiplayer;
using GameElements;
using Army;

namespace Control
{
    public class SetupFactory : MonoBehaviour, IControl
    {
        public GameObject factoryObject;
        public Factory factory;
        public Button okButton;
        public int detail;
        public float circleRadius;
        public float stripWidth;
        public Color goodColor;
        public Color badColor;
        public Vector2Int serverFactoryPosition;
        public Vector2Int clientFactoryPosition;
        public string popMessageStart;

        private bool good;
        private bool invokeReleaseEvent = false;
        private Vector2Int prevPosition;
        private Vector2Int currentPosition;
        private Mesh mesh;
        private new Transform transform;
        private bool active;

        private void Awake()
        {
            transform = GetComponent<Transform>();
            GameFlow.setupFactory = this;
            GameFlow.friendlyFactory = factory = Instantiate(factoryObject).GetComponent<Factory>();
            factory.type = UnitType.Friendly;
            GameFlow.enemyFactory = Instantiate(factoryObject).GetComponent<Factory>();
            GameFlow.enemyFactory.type = UnitType.Enemy;
            GameFlow.enemyFactory.gameObject.SetActive(false);
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            GameFlow.GenerateCircleMesh(mesh, circleRadius, circleRadius + stripWidth, detail);
            GUIUpdate();
            if (Socket.socketType == SocketType.Server)
            {
                factory.MoveToPosition(serverFactoryPosition);
                prevPosition = serverFactoryPosition;
            }
            else
            {
                factory.MoveToPosition(clientFactoryPosition);
                prevPosition = clientFactoryPosition;
            }
            GetComponent<MeshRenderer>().enabled = (false);

        }
        private void Start()
        {
            GameFlow.uiTutorialText.Pop(popMessageStart);
            StartCoroutine(StartFocusCoroutine());
        }
        IEnumerator StartFocusCoroutine()
        {
            GameFlow.cameraInput.active = false;
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(GameFlow.cameraControl.Focus(factory.position, 0.2f));
            GameFlow.cameraInput.active = true;
        }
        public void KeyCanceled()
        {
            invokeReleaseEvent = false;
            factory.MoveToPosition(prevPosition);
            transform.position = (Vector2)prevPosition;
            GUIUpdate();
        }
        public void KeyMoved(Vector2 position)
        {
            if (invokeReleaseEvent)
            {
                factory.MoveToPosition(Convert(position));
                transform.position = (Vector2)Convert(position);
                GUIUpdate();
            }
        }
        public void KeyPressed(Vector2 position)
        {
            GetComponent<MeshRenderer>().enabled = (true);
            invokeReleaseEvent = true;
            factory.MoveToPosition(Convert(position));
            transform.position = (Vector2)Convert(position);
            GUIUpdate();
        }
        public void KeyReleased(Vector2 position)
        {
            if (invokeReleaseEvent)
            {
                factory.MoveToPosition(Convert(position));
                if (factory.EvaluatePosition())
                {
                    prevPosition = factory.position;
                    transform.position = (Vector2)Convert(position);
                }
                else
                {
                    factory.MoveToPosition(prevPosition);
                    transform.position = (Vector2)prevPosition;
                }
                GUIUpdate();
            }
        }
        public void OnOKButton()
        {
            GameFlow.uIResourceCounter.Spawn();
            EventHandle.SetFriendlyFactory(prevPosition);
        }
        private void GUIUpdate()
        {
            bool good = factory.EvaluatePosition();
            SetCircle(good);
            okButton.interactable = good;
        }
        private void SetCircle(bool good)
        {
            if (good)
            {
                GameFlow.SetMeshColor(mesh, goodColor);
            }
            else
            {
                GameFlow.SetMeshColor(mesh, badColor);
            }
        }
        private Vector2Int Convert(Vector2 p)
        {
            return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
        }
        public bool GetActive()
        {
            return active;
        }
        public void SetActive(bool b)
        {
            active = b;
            invokeReleaseEvent = false;
            if (active)
            {
                factory.MoveToPosition(factory.position);
                GUIUpdate();
                gameObject.SetActive(true);
            }
            else
            {
                factory.MoveToPosition(prevPosition);
                transform.position = (Vector2)prevPosition;
                GUIUpdate();
                gameObject.SetActive(false);
            }
        }
    }
}