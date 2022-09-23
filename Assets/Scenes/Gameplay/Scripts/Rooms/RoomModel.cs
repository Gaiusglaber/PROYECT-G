using System;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.RoomSystem.View;

namespace ProyectG.Gameplay.RoomSystem.Room
{
    [System.Serializable]
    public class RoomModel
    {
        #region EXPOSED_FIELDS
        public string name = string.Empty;
        public string id = string.Empty;
        public int maxRoomSpace;
        [TextArea(2,10)] public string roomInfo = string.Empty;
        public Vector3 initialWorldPosition = Vector3.zero;
        public Vector3 viewPosition = Vector3.zero;

        public BuildModel buildAttach;
        #endregion

        #region PRIVATE_FIELDS
        private Action onRemoveBuild = null;
        private Action<Vector2> onBuild = null;

        private bool isEmpty = true;
        #endregion

        #region PROPERTIES
        public bool IsRoomEmpty { get { return IsEmpty(); } }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Action<Vector2> onBuild, Action onRemoveBuild)
        {
            this.onBuild = onBuild;
            this.onRemoveBuild = onRemoveBuild;
        }

        public RoomModel() { }

        public void SetBuild(BuildModel build)
        {
            buildAttach = build;
            onBuild?.Invoke(initialWorldPosition);

            isEmpty = false;
        }

        public void RemoveBuild()
        {
            isEmpty = true;

            buildAttach = null;
            onRemoveBuild?.Invoke();
        }

        public bool IsEmpty()
        {
            return isEmpty;
        }
        #endregion
    }

    [System.Serializable]
    public class BuildModel
    {
        public string buildingName;
        public Sprite buildingImage;
        public List<Machine> machines = new List<Machine>();
        public List<ResourceModel> viewResources = new List<ResourceModel>();
    }
}
