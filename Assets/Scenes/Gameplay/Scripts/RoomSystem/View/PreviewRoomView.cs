using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class PreviewRoomView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private RawImage renderImage = null;

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
        }
        #endregion
    }
}