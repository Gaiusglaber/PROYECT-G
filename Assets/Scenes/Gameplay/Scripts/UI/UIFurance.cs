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
    public Action<ItemView> OnProcessMaterial;
    public Action onCancelProcess = null;

    #region UNITY_CALLS
    void Start()
    {
        inputSlot.Init(prefabItemView, mainCanvas, default, false, ItemType.fuel);
        outputSlot.Init(prefabItemView, mainCanvas, default, false, ItemType.fuel);
    }

    void Update()
    {
        inputSlot.UpdateViewSlot(inventoryController.StackTake);
        outputSlot.UpdateViewSlot(inventoryController.StackTake);

        if (!IsFurnanceActive.Invoke())
        {
            Debug.Log("Input slot stackList: " + inputSlot.StackOfItemsView.Count);

            if (inputSlot.StackOfItemsView.Count > 0)
            {
                ItemView itemView = inputSlot.StackOfItemsView[0];
                OnProcessMaterial?.Invoke(itemView);
            }
        }
        else
        {
            if (!inputSlot.StackUpdated)
            {
                onCancelProcess?.Invoke();
                Debug.Log("Proceso cancelado");
            }
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void GenerateProcessedItem(ItemView itemFrom)
    {
        FuelItem itemToCreate = inventoryController.GetItemModelFromId(itemFrom.ItemType) as FuelItem;

        ItemModel finalItem = itemToCreate.itemResults[0];

        outputSlot.CreateAndAddItemsFromData(finalItem, 1);
    }

    public void ShowPanel(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnEndProcess()
    {
        if (inputSlot.StackOfItemsView.Count < 1)
            return;

        Destroy(inputSlot.StackOfItemsView[0].gameObject);
        inputSlot.StackOfItemsView.RemoveAt(0);
        inputSlot.UpdateTextOutStack();
    }
    #endregion

    #region PRIVATE_METHODS

    #endregion
}
