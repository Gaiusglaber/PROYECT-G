using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class UIFurance : MonoBehaviour
{

    public SlotInventoryView inputSlot;
    public SlotInventoryView outputSlot;
    public InventoryController inventoryController;
    [SerializeField] private GameObject prefabItemView = null;
    [SerializeField] private Canvas mainCanvas = null;

    void Start()
    {
        inputSlot.Init(prefabItemView, mainCanvas, default, false, ItemType.fuel);
        outputSlot.Init(prefabItemView, mainCanvas, default, false, ItemType.fuel);
    }

    void Update()
    {
        inputSlot.UpdateViewSlot(inventoryController.StackTake);
        outputSlot.UpdateViewSlot(inventoryController.StackTake);
        Debug.Log("cantidad del input slot: " + inputSlot.AmountOutStack);
    }

    public void ShowPanel(bool active)
    {
        gameObject.SetActive(active);
    }
}
