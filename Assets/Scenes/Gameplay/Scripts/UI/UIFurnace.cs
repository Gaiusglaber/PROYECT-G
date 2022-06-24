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
    [SerializeField] private GameObject panelFurnace = null;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Image progressFillProcess = null;
    [SerializeField] private Image progressFuelConsumption = null;
    #endregion

    #region PRIVATE_FIELDS
    private Func<bool> isFurnaceProcessing = null;
    private Func<bool> isFurnaceBurningFuel = null;

    private float durationProcess = 0f;

    private bool extraPositionsCreated = false;

    private List<SlotInventoryView> thisUiSlotsView = new List<SlotInventoryView>();

    private Vector2Int invalidPosition = new Vector2Int(-1, -1);
    #endregion

    #region ACTIONS
    public Action<ItemModel> OnProcessMaterial;
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

        thisUiSlotsView.Add(inputSlot);
        thisUiSlotsView.Add(outputSlot);
        thisUiSlotsView.Add(fuelSlot);

        extraPositionsCreated = false;

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

    public void GenerateProcessedItem(ItemModel itemFrom)
    {
        ItemModel finalItem = itemFrom.itemResults[0];
        inventoryController.GenerateItem(finalItem.itemId, outputSlot.GridPosition);
    }

    public void TogglePanel()
    {
        panelFurnace.SetActive(!panelFurnace.activeSelf);
        inventoryController.ToggleInventory();

        ThisIsAwfulButNeeded();        
    }

    public void OnEndProcess()
    {
        progressFillProcess.fillAmount = 0;

        inventoryController.RemoveItems(inputSlot.GridPosition,1);

        inputSlot.UpdateTextOutStack();
    }

    public void OnEndBurnOfFuel()
    {
        progressFuelConsumption.fillAmount = 0;

        inventoryController.RemoveItems(fuelSlot.GridPosition, 1);

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
            if (inventoryController.Model.GetSlot(inputSlot.GridPosition) == null)
                return;

            if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems.Count > 0)
            {
                if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0].itemResults.Count < 1)
                    return;
                
                Debug.Log("Item send to procees");

                ItemModel firstItem = inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0];

                OnProcessMaterial?.Invoke(firstItem);
            }
        }
    }

    private bool ProcessFuel()
    {
        if (!IsFurnaceBurning.Invoke())
        {
            if (inventoryController.Model.GetSlot(fuelSlot.GridPosition) == null)
                return false;

            if(inventoryController.Model.GetSlot(fuelSlot.GridPosition).StackOfItems.Count > 0)
            {
                Debug.Log("Fuel send to burn");

                FuelItem fuelBurning = inventoryController.Model.GetSlot(fuelSlot.GridPosition).StackOfItems[0] as FuelItem;

                OnProcessFuel?.Invoke(fuelBurning);

                return true;
            }            
        }

        return false;
    }

    private void StopFill()
    {
        progressFillProcess.fillAmount = 0;
    }

    private void ThisIsAwfulButNeeded()
    {
        if (panelFurnace.activeSelf)
        {
            if (extraPositionsCreated)
                return;

            //Ojo esta parte! Esto es muy feo pero tuve que hacerlo asi porque no encontraba forma de resolverlo
            //literalmente estoy "extendiendo" el inventario para poder interactuar con la UI. Eventualmente esto
            //tendriamos que cambiarlo u encontrar algo mas optimo. Btw por ahora me sirve. :)
            Debug.Log("Creado de posiciones extra del inventario");

            inventoryController.ExtendInventoryWithExtraSlots(80, 81, 80, 83, thisUiSlotsView);    //Puse 80/83 como para que sean slots imposibles de usar realmente

            inputSlot.SetSlotGridPosition(inventoryController.GetExtraSlotsFromInventory()[0].GridPosition);
            outputSlot.SetSlotGridPosition(inventoryController.GetExtraSlotsFromInventory()[1].GridPosition);
            fuelSlot.SetSlotGridPosition(inventoryController.GetExtraSlotsFromInventory()[2].GridPosition);

            Debug.Log("extra slots del horno 1: " + inventoryController.GetExtraSlotsFromInventory()[0]);
            Debug.Log("extra slots del horno 2: " + inventoryController.GetExtraSlotsFromInventory()[1]);
            Debug.Log("extra slots del horno 3: " + inventoryController.GetExtraSlotsFromInventory()[2]);

            extraPositionsCreated = true;
        }
        else
        {
            if (inventoryController.Model.GetSlot(inputSlot.GridPosition) == null ||
                inventoryController.Model.GetSlot(outputSlot.GridPosition) == null ||
                inventoryController.Model.GetSlot(fuelSlot.GridPosition) == null)
                return;
 

            if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems.Count > 0 ||
                inventoryController.Model.GetSlot(outputSlot.GridPosition).StackOfItems.Count > 0 ||
                inventoryController.Model.GetSlot(fuelSlot.GridPosition).StackOfItems.Count > 0)
                return;

            Debug.Log("Limpiado de posiciones extra del inventario");

            inventoryController.ClearExtraSlotsInventory();

            inputSlot.SetSlotGridPosition(invalidPosition);
            outputSlot.SetSlotGridPosition(invalidPosition);
            fuelSlot.SetSlotGridPosition(invalidPosition);

            extraPositionsCreated = false;
        }
    }
    #endregion
}
