using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.RoomSystem.Room;

using TMPro;
using System;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class RoomView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        public TMP_Text roomName = null;
        public RoomModel roomModel = null;
        public Vector3 positionInWorld = Vector3.zero;
        #endregion

        #region PRIVATE_FIELDS
        private Action onRemoveBuild = null;
        private Action<Vector2> onBuild = null;
        #endregion

        #region PROPERTIES
        public Action OnRemoveBuild { get { return onRemoveBuild; } set { onRemoveBuild = value; } }
        public Action<Vector2> OnBuild { get { return onBuild; } set { onBuild = value; } }
        #endregion

        #region PUBLIC_METHODS
        public void Init(RoomModel roomModel = null)
        {
            this.roomModel = roomModel;
            roomName.text = "Building: EMPTY";

            this.roomModel.Init(OnBuild, OnRemoveBuild);

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

        public void DestroyBuildInRoom()
        {
            if (roomModel == null)
            {
                Debug.Log("The room model is NULL");
                return;
            }

            roomModel.RemoveBuild();
        }
        #endregion
    }
}
