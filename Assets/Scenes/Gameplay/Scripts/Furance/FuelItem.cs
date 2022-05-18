using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class FuelItem : ItemModel
{
    public bool produced;
    public bool notProduced;
    public List<ItemModel> itemResults = new List<ItemModel>();
}
