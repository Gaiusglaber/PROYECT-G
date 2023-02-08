using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;

public class FuelItem : ItemModel
{
    [Header("FUEL SETTINGS")]
    public float timeToBurnOut;
    public float energyPerTime;
    public int energyGenerated;
    public int pollutantMaxEnergy;
}
