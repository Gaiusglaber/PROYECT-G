using UnityEngine;

namespace ProyectG.Gameplay.Objects.Inventory.Data
{
    [CreateAssetMenu(fileName = "FuelModel", menuName = "ScriptableObjects/Data/Items/FuelModel", order = 0)]
    public class FuelItem : ItemModel
    {
        [Header("FUEL SETTINGS")] 
        public float timeToBurnOut;
        public float energyPerTime;
        public int energyGenerated;
    }
}