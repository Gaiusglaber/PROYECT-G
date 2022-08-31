using ProyectG.Gameplay.RoomSystem.Room;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class RoomView : MonoBehaviour
    {
        public List<ResourceView> resourcesNeeded = new List<ResourceView>();
        public TMP_Text roomName = null;
        public TMP_Text energyCost = null;
        public RoomModel roomModel = null;
        public Vector3 positionInWorld = Vector3.zero;

        public void Init(RoomModel roomModel, List<(int,Sprite)> resourceData = null)
        {
            this.roomModel = roomModel;

            roomName.text = "Room " + roomModel.id;

            if (resourceData == null)
                return;

            for (int i = 0; i < resourcesNeeded.Count; i++)
            {
                resourcesNeeded[i].Init(resourceData[i].Item1, resourceData[i].Item2);
            }
        }
        
    }
}
