using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.RoomSystem.Room;

using TMPro;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class RoomView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        public TMP_Text roomName = null;
        public RoomModel roomModel = null;
        public Vector3 positionInWorld = Vector3.zero;
        #endregion

        #region PUBLIC_METHODS
        public void Init(RoomModel roomModel = null)
        {
            this.roomModel = roomModel;
            roomName.text = "Building: EMPTY";

            roomName.gameObject.SetActive(false);
        }

        public void BuildInRoom(BuildModel buildModel)
        {
            if(roomModel == null)
            {
                Debug.Log("The room model is NULL");
                return;
            }

            roomModel.SetBuild(buildModel);
        }
        #endregion
    }
}
