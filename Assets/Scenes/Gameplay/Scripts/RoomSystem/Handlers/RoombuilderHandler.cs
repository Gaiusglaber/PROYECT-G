using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;
using ProyectG.Gameplay.Controllers;
using ProyectG.Gameplay.RoomSystem.View;

namespace ProyectG.Gameplay.RoomSystem.Handler
{
    public class RoombuilderHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject feedbackView = null;
        [SerializeField] private RoombuilderView roombuilderView = null;
        [SerializeField] private PreviewRoomView previewRoom = null;
        #endregion

        #region PRIVATE_FIELDS
        private CameraController cameraHandler = null;
        private PlayerController playerController = null;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            feedbackView.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            feedbackView.SetActive(true);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if(Input.GetKey(KeyCode.E))
                {
                    Debug.Log("Pressed E");
                    roombuilderView.ToggleView(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            feedbackView.SetActive(false);
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(PlayerController player, CameraController camera)
        {
            playerController = player;
            cameraHandler = camera;
            
            if (roombuilderView == null)
                return;

            roombuilderView.Init();

            roombuilderView.OnViewToggle += (viewActive) => 
            {
                cameraHandler.ToggleAttachToPlayer(!viewActive);
                playerController.ToggleController(!viewActive);
            };

            roombuilderView.OnSelectedRoom += (position) =>
            {
                cameraHandler.MoveCamera(position);
            };
        }
        #endregion
    }
}