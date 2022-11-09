using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.RoomSystem.Room;

using ProyectG.Toolbox.Lerpers;
using TMPro;

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
        [SerializeField] private TMP_Text stateRoom = null;

        [Header("ROOM ANIMATIONS")]
        [SerializeField] private Vector2 hidedPreviewView = default;
        [SerializeField] private Vector2 showedPreviewView = default;
        #endregion

        #region PRIVATE_FIELDS
        private RoomView selectedRoom = null;
        private Camera refCamera = null;

        private Vector2Lerper positionLerper = null;
        private RectTransform rectTransform = null;

        private List<BuildView> allBuildsAviables = new List<BuildView>();
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if(positionLerper == null)
            {
                return;
            }

            if(positionLerper.On)
            {
                positionLerper.Update();

                rectTransform.anchoredPosition = positionLerper.CurrentValue;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Camera camera, List<BuildModel> aviableBuildings, Action<string, Action<bool>> onBuildPressed, Action<string ,Action<bool>> onDestroyPressed)
        {
            refCamera = camera;

            for (int i = 0; i < aviableBuildings.Count; i++)
            {
                if (aviableBuildings[i] != null)
                {
                    BuildView newResource = Instantiate(prefabBuildView, holderBuildings);

                    newResource.InitBuildView(aviableBuildings[i].buildingImage, aviableBuildings[i].buildingName, 
                        aviableBuildings[i].viewResources, onBuildPressed, onDestroyPressed);

                    allBuildsAviables.Add(newResource);
                }
            }

            rectTransform = transform as RectTransform;

            positionLerper = new Vector2Lerper(.25f, Vector2Lerper.SMOOTH_TYPE.EASE_IN);

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

            string roomType = !selectedRoom.roomModel.isOutsideRoom ? "<color=yellow>Room is for machines.</color>"
                : "<color=yellow>The room is for organics.</color>";

            stateRoom.text = roomState;

            infoRoom.text = roomType + '\n' + '\n' + selectedRoom.roomModel.roomInfo;
        }

        public BuildView GetBuildById(string buildID)
        {
            BuildView build = allBuildsAviables.Find(build => build.NameBuild == buildID);
            return build;
        }

        public RoomView GetSelectedRoom()
        {
            return selectedRoom;
        }

        public void TogglePreview(bool state)
        {
            if (!state)
            {
                rectTransform.anchoredPosition = hidedPreviewView;
            }

            positionLerper.SetValues(rectTransform.anchoredPosition, state ? showedPreviewView : hidedPreviewView, true);
            //holderPreview.SetActive(state);
        }
        #endregion
    }
}