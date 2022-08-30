using ProyectG.Gameplay.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.RoomSystem.Room
{
    public class RoomModel
    {
        public int cost = 0;
        public bool isBuyed = false;
        public Vector3 position = Vector3.zero;
        public List<Machine> machines = new List<Machine>();
    }
}
