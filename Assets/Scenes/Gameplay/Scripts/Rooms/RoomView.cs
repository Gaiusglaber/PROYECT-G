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
        public TMP_Text energyCost = null;
        public RoomModel dataAttach = null;
        public Vector3 positionInWorld = Vector3.zero;
    }
}
