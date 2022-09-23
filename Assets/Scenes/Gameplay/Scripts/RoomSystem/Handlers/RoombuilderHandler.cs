using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;
using ProyectG.Gameplay.Controllers;
using ProyectG.Gameplay.RoomSystem.View;
using ProyectG.Gameplay.RoomSystem.Room;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects;

namespace ProyectG.Gameplay.RoomSystem.Handler
{
    public class RoombuilderHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject feedbackView = null;
        [SerializeField] private RoombuilderView roombuilderView = null;
        [SerializeField] private PreviewRoomView previewRoom = null;

        [SerializeField] private List<BuildModel> allBuildingAviables = null;
        #endregion

        #region PRIVATE_FIELDS
        private InventoryController inventoryController = null;
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
        public void Init(PlayerController player, CameraController camera, InventoryController playerInventory)
        {
            inventoryController = playerInventory;
            playerController = player;
            cameraHandler = camera;
            
            if (roombuilderView == null)
                return;

            roombuilderView.Init(OnBuildSomething, OnDestroyBuilding);

            previewRoom.Init(camera.MainCamera, allBuildingAviables, OnBuildInSelectedRoom);

            roombuilderView.OnViewToggle += (viewActive) => 
            {
                cameraHandler.ToggleAttachToPlayer(!viewActive);
                playerController.ToggleController(!viewActive);
            };

            roombuilderView.OnSelectedRoom += (position) =>
            {
                cameraHandler.MoveCamera(position);

                previewRoom.SetPreviewRoom(roombuilderView.GetSelectedRoom());
            };

            roombuilderView.OnUnselectRoom += ()=>
            {
                previewRoom.SetPreviewRoom(null);
            };
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnBuildInSelectedRoom(string name, Action<bool> stateOperation)
        {
            //We find the build to make on the actual selected room
            BuildModel buildToCreate = allBuildingAviables.Find(build => build.buildingName == name);

            if(buildToCreate == null)
            {
                Debug.Log("There is not room aviable with that name.");
                return;
            }

            //We get the selected room from preview
            RoomView actualRoomInPreview = previewRoom.GetSelectedRoom();

            Vector3 positionToBuild = actualRoomInPreview.roomModel.initialWorldPosition;

            if(buildToCreate.machines.Count < 1)
            {
                Debug.Log("The build has not machines attach. se we cant create nothing in here.");
                return;
            }

            bool canMakeBuild = false;
            
            //We check with this if the player has enough resources to build this machine

            for (int i = 0; i < buildToCreate.viewResources.Count; i++)
            {
                if (buildToCreate.viewResources[i] != null)
                {
                    if(inventoryController.HasEnoughOfThisItem(buildToCreate.viewResources[i].item, buildToCreate.viewResources[i].amount))
                    {
                        canMakeBuild = true;
                    }
                }
            }

            //If the player doesn´t have enough resources we DO NOT build
            if(!canMakeBuild)
            {
                Debug.Log("You don´t have enough resources to build that! Go farm more");
                stateOperation?.Invoke(false); //We notify the final state of this build operation, false if failed.
                return;
            }

            //Else we can go and consume the resources from the inventory and build the machine
            for (int i = 0; i < buildToCreate.viewResources.Count; i++)
            {
                if (buildToCreate.viewResources[i] != null)
                {
                    inventoryController.ConsumeItems(buildToCreate.viewResources[i].item, buildToCreate.viewResources[i].amount);
                }
            }

            //We create the machine/farm
            Machine building = Instantiate(buildToCreate.machines[0], positionToBuild, Quaternion.identity);

            //We pass the data to the room, to know that this room has now this machine/build
            actualRoomInPreview.BuildInRoom(buildToCreate);

            //Update text state
            previewRoom.SetPreviewRoom(actualRoomInPreview);

            //We notify the final state of this build operation, true if succed. 
            stateOperation?.Invoke(true);
        }

        private void OnDestroyBuildOnSelectedRoom(string name, Action<bool> stateOperation)
        {
            //We get the selected room from preview
            RoomView actualRoomInPreview = previewRoom.GetSelectedRoom();

            //We find the build to make on the actual selected room
            BuildModel buildToRemove = actualRoomInPreview.roomModel.buildAttach;

            if (buildToRemove == null)
            {
                Debug.Log("There is not room aviable with that name.");
                return;
            } 
        }

        private void OnBuildSomething(Vector2 buildPosition)
        {
            Debug.Log("New build created!");
        }

        private void OnDestroyBuilding()
        {
            Debug.Log("The build has been destroyed!");
        }
        #endregion
    }
}