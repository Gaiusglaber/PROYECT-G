using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

using ProyectG.Toolbox.Lerpers;

public class UIFurance : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private SlotInventoryView inputSlot;
    [SerializeField] private SlotInventoryView outputSlot;
    [SerializeField] private SlotInventoryView fuelSlot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private GameObject prefabItemView = null;
    [SerializeField] private GameObject panelFurance = null;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Image progressFillProcess = null;
    [SerializeField] private Image progressFuelConsumption = null;
    #endregion

    #region PRIVATE_FIELDS
    private Func<bool> isFurnanceActive = null;

    private float durationProcess = 0f;

    private Vector2Int invalidPosition = new Vector2Int(-1, -1);
    #endregion

    #region ACTIONS
    public Action<ItemView> OnProcessMaterial;
    public Action onCancelProcess = null;
    #endregion

    #region PROPERTIES
    public InventoryController InverntoryController { get { return inventoryController; } }
    public Func<bool> IsFurnanceActive { set { isFurnanceActive = value; } get { return isFurnanceActive; } }
    #endregion

    #region UNITY_CALLS
    void Start()
    {
        inputSlot.Init(prefabItemView, mainCanvas, invalidPosition, false);
        outputSlot.Init(prefabItemView, mainCanvas, invalidPosition, false);
        fuelSlot.Init(prefabItemView, mainCanvas, invalidPosition, false, ItemType.fuel);

        onCancelProcess += StopFill;
    }

    void Update()
    {
        inputSlot.UpdateViewSlot(inventoryController.StackTake);
        outputSlot.UpdateViewSlot(inventoryController.StackTake);

        if (!IsFurnanceActive.Invoke())
        {
            if(panelFurance.activeSelf)
            {                
                if (inputSlot.StackOfItemsView.Count > 0)
                {
                    ItemView itemView = inputSlot.StackOfItemsView[0];
                    OnProcessMaterial?.Invoke(itemView);
                }
            }
            else
            {
                if (inputSlot.AuxStackOfItems.Count > 0)
                {
                    ItemView itemView = inputSlot.AuxStackOfItems[0];
                    OnProcessMaterial?.Invoke(itemView);
                }
            }
        }
        else
        {
            if (!panelFurance.activeSelf)
                return;

            if (!inputSlot.StackUpdated)
            {
                onCancelProcess?.Invoke();
                Debug.Log("Proceso cancelado");
            }
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetDurationProcess(float timeToBurn)
    {
        durationProcess = timeToBurn;
    }

    public void GenerateProcessedItem(ItemView itemFrom)
    {
        FuelItem itemToCreate = inventoryController.GetItemModelFromId(itemFrom.ItemType) as FuelItem;

        ItemModel finalItem = itemToCreate.itemResults[0];

        outputSlot.CreateAndAddItemsFromData(finalItem, 1);
    }

    public void TogglePanel()
    {
        FillAuxiliarList(inputSlot.StackOfItemsView);

        panelFurance.SetActive(!panelFurance.activeSelf);
        inventoryController.ToggleInventory();
    }

    public void OnEndProcess()
    {
        progressFillProcess.fillAmount = 0;

        if(panelFurance.activeSelf)
        {
            if (inputSlot.StackOfItemsView.Count < 1)
                return;

            Destroy(inputSlot.StackOfItemsView[0].gameObject);
            inputSlot.StackOfItemsView.RemoveAt(0);
        }
        else
        {
            if (inputSlot.AuxStackOfItems.Count < 1)
                return;

            Destroy(inputSlot.AuxStackOfItems[0].gameObject);
            inputSlot.AuxStackOfItems.RemoveAt(0);
        }
        
        inputSlot.UpdateTextOutStack();
    }

    public void UpdateProgressFill(float actualTime)
    {
        float fillValue = (actualTime * 100f) / durationProcess;
        fillValue = fillValue / 100f;

        progressFillProcess.fillAmount = Mathf.MoveTowards(progressFillProcess.fillAmount, fillValue, Time.deltaTime);
    }
    #endregion

    #region PRIVATE_METHODS
    private void FillAuxiliarList(List<ItemView> listOfItems)
    {
        if (listOfItems.Count < 1)
            return;

        for (int i = 0; i < listOfItems.Count; i++)
        {
            if(!inputSlot.AuxStackOfItems.Contains(listOfItems[i]))
            {
                inputSlot.AuxStackOfItems.Add(listOfItems[i]);
            }
        }
    }

    private void StopFill()
    {
        progressFillProcess.fillAmount = 0;
    }
    #endregion
}
