using System;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.RoomSystem.Room;
using ProyectG.Gameplay.Objects;

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

        #region PRIVATE_FIELDS
        private Action onRemoveBuild = null;
        private Action onBuild = null;

        private Machine machineCreated = null;
        #endregion

        #region PROPERTIES
        public Action OnRemoveBuild { get { return onRemoveBuild; } set { onRemoveBuild = value; } }
        public Action OnBuild { get { return onBuild; } set { onBuild = value; } }
        #endregion

        #region PUBLIC_METHODS
        public void Init(RoomModel roomModel = null)
        {
            this.roomModel = roomModel;
            roomName.text = "Building: EMPTY";

            this.roomModel.Init(OnBuild, OnRemoveBuild);

            roomName.gameObject.SetActive(false);
        }

        public void BuildInRoom(BuildModel buildModel, Machine machineCreated = null)
        {
            if(roomModel == null)
            {
                Debug.Log("The room model is NULL");
                return;
            }

            roomModel.SetBuild(buildModel);

            roomName.text = buildModel.buildingName;

            //Instantiate(buildModel.backgroundRoom, buildModel.)

            this.machineCreated = machineCreated;
        }

        public void DestroyBuildInRoom()
        {
            if (roomModel == null)
            {
                Debug.Log("The room model is NULL");
                return;
            }

            if(machineCreated != null)
            {
                Destroy(machineCreated.gameObject);
            }

            roomName.text = "Building: EMPTY";

            roomModel.RemoveBuild();
        }
        #endregion
    }
}
