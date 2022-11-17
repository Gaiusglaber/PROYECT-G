using System;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Common.Modules.Audio.Channels.Sound;

using ProyectG.Player.Controller;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Controllers;
using ProyectG.Gameplay.RoomSystem.View;
using ProyectG.Gameplay.RoomSystem.Room;
using ProyectG.Gameplay.Objects.Inventory.Controller;

namespace ProyectG.Gameplay.RoomSystem.Handler
{
    public class RoombuilderHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject feedbackView = null;
        [SerializeField] private RoombuilderView roombuilderView = null;
        [SerializeField] private PreviewRoomView previewRoom = null;
        [SerializeField] private SoundHandlerChannel soundsChannel = null;

        [SerializeField] private List<BuildModel> allBuildingAviables = null;
        [SerializeField] private List<BaseView> allBaseViews = null;
        [SerializeField] private string idTreeFarm = null;
        [SerializeField] private string idRoomForTrees = null;

        [Header("Resources")]
        [SerializeField, Range(0, 100)] private float porcentOnDestroyBuild = 100f;
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

            previewRoom.Init(camera.MainCamera, allBuildingAviables, OnBuildInSelectedRoom, OnDestroyBuildOnSelectedRoom);

            roombuilderView.Init(OnBuildSomething, OnDestroyBuilding, () => { previewRoom.TogglePreview(false); });

            roombuilderView.OnViewToggle += (viewActive) => 
            {
                cameraHandler.ToggleAttachToPlayer(!viewActive);
                playerController.ToggleController(!viewActive);
            };

            roombuilderView.OnSelectedRoom += (position) =>
            {
                roombuilderView.ToggleInput(false);

                cameraHandler.MoveCamera(position, 
                    onMoveEnded: () => 
                    {
                        roombuilderView.ToggleInput(true);                       
                    });

                previewRoom.SetPreviewRoom(roombuilderView.GetSelectedRoom());
            };

            roombuilderView.OnUnselectRoom += ()=>
            {
                previewRoom.SetPreviewRoom(null);
            };

            PreCreateTreeFarm();
        }
        #endregion

        #region PRIVATE_METHODS 
        private void PreCreateTreeFarm()
        {
            BuildModel treeFarm = allBuildingAviables.Find(build => build.buildingName == idTreeFarm);
            BuildView actualRoomInPreview = previewRoom.GetBuildById(idTreeFarm);
            RoomView treeRoom = roombuilderView.GetRoomById(idRoomForTrees);

            Vector3 positionToBuild = treeRoom.roomModel.initialWorldPosition;

            Machine building = Instantiate(treeFarm.machines[0], positionToBuild, Quaternion.identity);

            treeRoom.BuildInRoom(treeFarm, building);

            actualRoomInPreview.OnBuildCreated(true);
        }

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

            if(!actualRoomInPreview.roomModel.IsRoomEmpty)
            {
                Debug.Log("The selected room already has a build in here.");
                return;
            }

            bool canMakeBuild = false;
            bool wasNotEnoughResources = false;

            //We check with this if the player has enough resources to build this machine

            int amountResourcesPass = 0;

            for (int i = 0; i < buildToCreate.viewResources.Count; i++)
            {
                if (buildToCreate.viewResources[i] != null)
                {
                    if (inventoryController.HasEnoughOfThisItem(buildToCreate.viewResources[i].item, buildToCreate.viewResources[i].amount))
                    {
                        amountResourcesPass++;
                    }
                }
            }

            if(amountResourcesPass == buildToCreate.viewResources.Count)
            {
                canMakeBuild = true;
            }

            if (!canMakeBuild)
            {
                wasNotEnoughResources = true;
            }
            else
            {
                if (buildToCreate.isMachine)
                {
                    canMakeBuild = !actualRoomInPreview.roomModel.isOutsideRoom;
                }
                else
                {
                    canMakeBuild = actualRoomInPreview.roomModel.isOutsideRoom;
                }
            }

            //If the player doesn´t have enough resources we DO NOT build
            if (!canMakeBuild)
            {
                Debug.Log("You don´t have enough resources to build that! Go farm more");

                string feedback = wasNotEnoughResources ? "You don´t have enough resources!" : "You cannot build that on this room.";

                roombuilderView.ShowFeedbackBuild(feedback, false);
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

            BaseView machineUI = allBaseViews.Find(view => view.IdView == name);
            
            //Tiene UI la maquina?
            if(machineUI != null)
            {
                building.Init(machineUI);
            }

            //We pass the data to the room, to know that this room has now this machine/build
            actualRoomInPreview.BuildInRoom(buildToCreate, building);

            //Update text state
            previewRoom.SetPreviewRoom(actualRoomInPreview);

            //We notify the final state of this build operation, true if succed. 
            roombuilderView.ShowFeedbackBuild("Build succed!",true);
            stateOperation?.Invoke(true);
        }

        private void OnDestroyBuildOnSelectedRoom(string name, Action<bool> stateOperation)
        {
            //We get the selected room from preview
            RoomView actualRoomInPreview = previewRoom.GetSelectedRoom();

            //We find the build to make on the actual selected room
            BuildModel buildToRemove = allBuildingAviables.Find(build => build.buildingName == name);

            if (buildToRemove == null)
            {
                Debug.Log("There is not room aviable with that name.");
                stateOperation?.Invoke(false);
                return;
            }

            if(buildToRemove != actualRoomInPreview.roomModel.buildAttach)
            {
                Debug.Log("There is not room aviable with that name.");
                stateOperation?.Invoke(false);
                return;
            }

            for (int i = 0; i < buildToRemove.viewResources.Count; i++)
            {
                if (buildToRemove.viewResources[i] != null)
                {
                    inventoryController.RetrivePorcentOfConsumed(buildToRemove.viewResources[i].item, buildToRemove.viewResources[i].amount, porcentOnDestroyBuild);                   
                }
            }

            actualRoomInPreview.DestroyBuildInRoom();

            //Update text state
            previewRoom.SetPreviewRoom(actualRoomInPreview);

            stateOperation?.Invoke(true);
        }

        private void OnBuildSomething()
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