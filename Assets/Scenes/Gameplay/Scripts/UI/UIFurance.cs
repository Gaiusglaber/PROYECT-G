using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;

public class UIFurance : MonoBehaviour
{

    public SlotInventoryView inputSlot;
    public SlotInventoryView outputSlot;
    public InventoryController inventoryController;

    void Start()
    {
        
    }

    void Update()
    {
        //if(inputSlot.)
    }

    public void ShowPanel(bool active)
    {
        gameObject.SetActive(active);
    }
}
