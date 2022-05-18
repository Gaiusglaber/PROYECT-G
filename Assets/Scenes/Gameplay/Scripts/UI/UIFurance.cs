using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private Func<bool> isFurnanceActive = null;

    public Func<bool> IsFurnanceActive { set { isFurnanceActive = value; } get { return isFurnanceActive; } }
    public Action<ItemView, Action> OnProcessMaterial;

    void Start()
    {
        inputSlot.Init(prefabItemView, mainCanvas, default, false, ItemType.fuel);
        outputSlot.Init(prefabItemView, mainCanvas, default, false, ItemType.fuel);
    }

    void Update()
    {
        inputSlot.UpdateViewSlot(inventoryController.StackTake);
        outputSlot.UpdateViewSlot(inventoryController.StackTake);

        if (!isFurnanceActive.Invoke())
        {
            if (inputSlot.StackOfItemsView.Count > 0)
            {
                Debug.Log("entro!");
                ItemView itemView = inputSlot.StackOfItemsView[inputSlot.StackOfItemsView.Count - 1];
                OnProcessMaterial?.Invoke(itemView, ()=> {
                    Destroy(inputSlot.StackOfItemsView[inputSlot.StackOfItemsView.Count - 1].gameObject);
                    inputSlot.StackOfItemsView.RemoveAt(inputSlot.StackOfItemsView.Count - 1);
                });
            }
        }
        Debug.Log("cantidad del input slot: " + inputSlot.AmountOutStack);
    }

    public void ShowPanel(bool active)
    {
        gameObject.SetActive(active);
    }
}
