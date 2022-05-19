using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

using ProyectG.Toolbox.Lerpers;

public class UIFurnace : MonoBehaviour
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
    private Func<bool> isFurnaceProcessing = null;
    private Func<bool> isFurnaceBurningFuel = null;

    private float durationProcess = 0f;

    private Vector2Int invalidPosition = new Vector2Int(-1, -1);
    #endregion

    #region ACTIONS
    public Action<ItemView> OnProcessMaterial;
    public Action<FuelItem> OnProcessFuel;
    public Action onCancelProcess = null;
    #endregion

    #region PROPERTIES
    public InventoryController InverntoryController { get { return inventoryController; } }
    public Func<bool> IsFurnaceProcessing { set { isFurnaceProcessing = value; } get { return isFurnaceProcessing; } }
    public Func<bool> IsFurnaceBurning { set { isFurnaceBurningFuel = value; } get { return isFurnaceBurningFuel; } }
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
        fuelSlot.UpdateViewSlot(InverntoryController.StackTake);

        ProcessMaterials();

        ProcessFuel();
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetDurationProcess(float timeToBurn)
    {
        durationProcess = timeToBurn;
    }

    public void GenerateProcessedItem(ItemView itemFrom)
    {
        ItemModel finalItem = inventoryController.GetItemModelFromId(itemFrom.ItemType).itemResults[0];
        outputSlot.CreateAndAddItemsFromData(finalItem, 1);
    }

    public void TogglePanel()
    {
        FillAuxiliarList(inputSlot.StackOfItemsView);
        FillAuxiliarListFuel(fuelSlot.StackOfItemsView);

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

    public void OnEndBurnOfFuel()
    {
        progressFuelConsumption.fillAmount = 1;

        if (panelFurance.activeSelf)
        {
            if (fuelSlot.StackOfItemsView.Count < 1)
                return;

            Destroy(fuelSlot.StackOfItemsView[0].gameObject);
            fuelSlot.StackOfItemsView.RemoveAt(0);
        }
        else
        {
            if (fuelSlot.AuxStackOfItems.Count < 1)
                return;

            Destroy(fuelSlot.AuxStackOfItems[0].gameObject);
            fuelSlot.AuxStackOfItems.RemoveAt(0);
        }

        fuelSlot.UpdateTextOutStack();
    }

    public void UpdateProgressFill(float actualTime)
    {
        float fillValue = (actualTime * 100f) / durationProcess;
        fillValue = fillValue / 100f;

        progressFillProcess.fillAmount = Mathf.MoveTowards(progressFillProcess.fillAmount, fillValue, Time.deltaTime);
    }

    public void UpdateFillFuel(float actualTime, float maxLifeTimeFuel)
    {
        float fillValue = (actualTime * 100f) / maxLifeTimeFuel;
        fillValue = fillValue / 100f;

        progressFuelConsumption.fillAmount = Mathf.MoveTowards(progressFuelConsumption.fillAmount, fillValue, Time.deltaTime);
    }
    #endregion

    #region PRIVATE_METHODS
    private void ProcessMaterials()
    {
        if (!IsFurnaceProcessing.Invoke())
        {
            if (panelFurance.activeSelf)
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

    private bool ProcessFuel()
    {
        if(!IsFurnaceBurning.Invoke())
        {
            if (panelFurance.activeSelf)
            {
                if (fuelSlot.StackOfItemsView.Count > 0)
                {
                    ItemView itemView = fuelSlot.StackOfItemsView[0];
                    FuelItem fuelBurning = inventoryController.GetItemModelFromId(itemView.ItemType) as FuelItem;

                    OnProcessFuel?.Invoke(fuelBurning);

                    return true;
                }
            }
            else
            {
                if (fuelSlot.AuxStackOfItems.Count > 0)
                {
                    ItemView itemView = fuelSlot.AuxStackOfItems[0];
                    FuelItem fuelBurning = inventoryController.GetItemModelFromId(itemView.ItemType) as FuelItem;

                    OnProcessFuel?.Invoke(fuelBurning);

                    return true;
                }
            }

            return false;
        }
        else
        {
            return true;
        }
    }

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

    private void FillAuxiliarListFuel(List<ItemView> listOfItems)
    {
        if (listOfItems.Count < 1)
            return;

        for (int i = 0; i < listOfItems.Count; i++)
        {
            if (!fuelSlot.AuxStackOfItems.Contains(listOfItems[i]))
            {
                fuelSlot.AuxStackOfItems.Add(listOfItems[i]);
            }
        }
    }

    private void StopFill()
    {
        progressFillProcess.fillAmount = 0;
    }
    #endregion
}
