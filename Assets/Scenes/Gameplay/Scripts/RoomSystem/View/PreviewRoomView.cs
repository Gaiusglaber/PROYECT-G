using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.RoomSystem.Room;

using TMPro;
using System;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class PreviewRoomView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject holderPreview = null;
        [SerializeField] private Transform holderBuildings = null;
        [SerializeField] private BuildView prefabBuildView = null;
        [SerializeField] private RawImage renderImage = null;
        [SerializeField] private TMP_Text infoRoom = null;
        #endregion

        #region PRIVATE_FIELDS
        private RoomView selectedRoom = null;
        private Camera refCamera = null;

        private List<BuildView> allBuildsAviables = new List<BuildView>();
        #endregion


        #region PUBLIC_METHODS
        public void Init(Camera camera, List<BuildModel> aviableBuildings, Action<string, Action<bool>> onBuildPressed)
        {
            refCamera = camera;

            for (int i = 0; i < aviableBuildings.Count; i++)
            {
                if (aviableBuildings[i] != null)
                {
                    BuildView newResource = Instantiate(prefabBuildView, holderBuildings);

                    newResource.InitBuildView(aviableBuildings[i].buildingImage, aviableBuildings[i].buildingName, aviableBuildings[i].viewResources, onBuildPressed);

                    allBuildsAviables.Add(newResource);
                }
            }

            TogglePreview(false);
        }

        public void SetPreviewRoom(RoomView selectedRoom)
        {
            if(selectedRoom == null)
            {
                TogglePreview(false);
                return;
            }
            else
            {
                TogglePreview(true);
            }

            this.selectedRoom = selectedRoom;

            if (selectedRoom.roomModel == null)
            {
                return;
            }

            string roomState = selectedRoom.roomModel.IsRoomEmpty ? "<color=green>Room is free to build.</color>" 
                : "<color=red>The room is already ocuped.</color>";

            infoRoom.text = roomState + '\n' + '\n' + selectedRoom.roomModel.roomInfo;
        }

        public RoomView GetSelectedRoom()
        {
            return selectedRoom;
        }

        public void TogglePreview(bool state)
        {
            holderPreview.SetActive(state);
        }
        #endregion
    }
}