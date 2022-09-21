using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class PreviewRoomView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private RawImage renderImage = null;
        [SerializeField] private TMP_Text infoRoom = null;
        #endregion

        #region PRIVATE_FIELDS
        private RoomView selectedRoom = null;
        private Camera refCamera = null;
        #endregion


        #region PUBLIC_METHODS
        public void Init(Camera camera)
        {
            refCamera = camera;
        }

        public void SetPreviewRoom(RoomView selectedRoom)
        {
            this.selectedRoom = selectedRoom;

            if(selectedRoom.roomModel == null)
            {
                return;
            }

            string roomState = selectedRoom.roomModel.IsRoomEmpty ? "<color=green>Room is free to build.</color>" 
                : "<color=red>The room is already ocuped.</color>";

            infoRoom.text = roomState + '\n' + '\n' + selectedRoom.roomModel.roomInfo;
        }
        #endregion
    }
}