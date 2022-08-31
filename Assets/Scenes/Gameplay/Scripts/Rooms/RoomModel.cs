using ProyectG.Gameplay.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.RoomSystem.Room
{
    [CreateAssetMenu(fileName = "RoomModel", menuName = "Rooms/RoomModel", order = 0)]
    public class RoomModel : ScriptableObject
    {
        public string id = string.Empty;
        public int cost = 0;
        public bool isBuyed = false;
        public Vector3 worldPosition = Vector3.zero;
        public Vector3 viewPosition = Vector3.zero;
        public List<Machine> machines = new List<Machine>();
    }
}
