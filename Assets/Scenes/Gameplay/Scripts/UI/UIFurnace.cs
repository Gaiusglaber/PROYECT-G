using System;

using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.Controller;

public class UIFurnace : BaseView
{
    #region EXPOSED_FIELDS
    [SerializeField] private MachineSlotView inputSlot;
    [SerializeField] private MachineSlotView outputSlot;
    [SerializeField] private MachineSlotView fuelSlot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ItemView prefabItemView = null;
    [SerializeField] private GameObject panelFurnace = null;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Image progressFillProcess = null;
    [SerializeField] private Image progressFuelConsumption = null;
    [SerializeField] private EnergyHandler energyHandler = null;
    #endregion

    #region PRIVATE_FIELDS
    private Func<bool> isFurnaceProcessing = null;
    private Func<bool> isFurnaceBurningFuel = null;

    private float durationProcess = 0f;
    #endregion

    #region ACTIONS
    public Action<ItemModel> OnProcessMaterial;
    public Action<FuelItem> OnProcessFuel;
    public Action onCancelProcess = null;
    public Action ActivateSecondBar = null;
    #endregion

    #region PROPERTIES
    public InventoryController InverntoryController { get { return inventoryController; } }
    public Func<bool> IsFurnaceProcessing { set { isFurnaceProcessing = value; } get { return isFurnaceProcessing; } }
    public Func<bool> IsFurnaceBurning { set { isFurnaceBurningFuel = value; } get { return isFurnaceBurningFuel; } }
    #endregion

    #region UNITY_CALLS
    void Start()
    {
        inputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);
        outputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);
        fuelSlot.Init(mainCanvas,inventoryController.OnHoverSelection, ItemType.fuel);

        inventoryController.OnInteractionChange += inputSlot.SetOnInteractionInventoryChange;
        inventoryController.OnInteractionChange += outputSlot.SetOnInteractionInventoryChange;
        inventoryController.OnInteractionChange += fuelSlot.SetOnInteractionInventoryChange;

        onCancelProcess += StopFill;
    }

    void Update()
    {
        ProcessMaterials();

        ProcessFuel();
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetDurationProcess(float timeToBurn)
    {
        durationProcess = timeToBurn;
    }

    public void GenerateProcessedItem(ItemModel itemFrom, Action<bool> canFinishSequence)
    {
        ItemModel finalItem = itemFrom.itemResults[0];
        
        ConsumePollutant(itemFrom);

        if(outputSlot == null)
        {
            Debug.LogWarning("Failed to generat result for the procesed item, the output slot is NULL");
            return;
        }

        bool outputIsEmpty = false;
        ItemModel firstItemOnOutput = null;

        if (inventoryController.StackTake)
        {
            outputIsEmpty = outputSlot.StackOfItems.Stack.Count < 1;

            if (!outputIsEmpty)
            {
                firstItemOnOutput = inventoryController.GetItemModelFromId(outputSlot.StackOfItems.Stack[0].ItemType);
            }
        }
        else
        {
            outputIsEmpty = outputSlot.ObjectsAttach.Count < 1;

            if (!outputIsEmpty)
            {
                firstItemOnOutput = inventoryController.GetItemModelFromId(outputSlot.ObjectsAttach[0].ItemType);
            }
        }

        if (!outputIsEmpty && firstItemOnOutput != null)
        {
            if(finalItem.itemId != firstItemOnOutput.itemId)
            {
                canFinishSequence?.Invoke(false);
                return;
            }
        }

        ItemView newItem = Instantiate(prefabItemView, outputSlot.SlotPosition, Quaternion.identity, outputSlot.transform);
        newItem.GenerateItem(mainCanvas, null, outputSlot, finalItem, inventoryController.Model.SiwtchItemsOnSlots, inventoryController.OnRemoveItems, inventoryController.OnAddItems);

        if (inventoryController.StackTake)
        {
            outputSlot.AddItemToStack(newItem);
        }
        else
        {
            outputSlot.AddItemToSlot(newItem);
        }

        canFinishSequence?.Invoke(true);
    }

    public override void TogglePanel()
    {
        panelFurnace.SetActive(!panelFurnace.activeSelf);
        inventoryController.ToggleInventory();
    }

    public void OnEndProcess()
    {
        progressFillProcess.fillAmount = 0;
    }

    public void OnEndBurnOfFuel()
    {
        progressFuelConsumption.fillAmount = 0;
    }
    
    public void OnConsumeItem()
    {
        ItemView itemToRemove = null;

        if (inventoryController.StackTake)
        {
            itemToRemove = inputSlot.StackOfItems.Stack[0];
            inputSlot.RemoveItemFromStack(itemToRemove);
        }
        else
        {
            itemToRemove = inputSlot.ObjectsAttach[0];
            inputSlot.RemoveItemFromSlot(itemToRemove);
        }
    }

    public void OnConsumeFuel()
    {
        ItemView fuelItem = null;

        if (inventoryController.StackTake)
        {
            fuelItem = fuelSlot.StackOfItems.Stack[0];
            fuelSlot.RemoveItemFromStack(fuelItem);
        }
        else
        {
            fuelItem = fuelSlot.ObjectsAttach[0];
            fuelSlot.RemoveItemFromSlot(fuelItem);
        }
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
            if(inputSlot != null)
            {
                ItemModel firstItem = null;

                if(inventoryController.StackTake)
                {
                    if(inputSlot.StackOfItems.Stack.Count > 0)
                    {
                        firstItem = inventoryController.GetItemModelFromId(inputSlot.StackOfItems.Stack[0].ItemType);
                    }
                }
                else
                {
                    if (inputSlot.ObjectsAttach.Count > 0)
                    {
                        firstItem = inventoryController.GetItemModelFromId(inputSlot.ObjectsAttach[0].ItemType);
                    }
                }

                if (firstItem != null)
                {
                    OnProcessMaterial?.Invoke(firstItem);
                }
            }
        }
    }

    private bool ProcessFuel()
    {
        if (!IsFurnaceBurning.Invoke())
        {           
            if(fuelSlot != null)
            {
                FuelItem fuelToBurn = null;

                if(inventoryController.StackTake)
                {
                    if(fuelSlot.StackOfItems.Stack.Count > 0)
                    {
                        fuelToBurn = inventoryController.GetItemModelFromId(fuelSlot.StackOfItems.Stack[0].ItemType) as FuelItem;
                    }
                }
                else
                {
                    if(fuelSlot.ObjectsAttach.Count > 0)
                    {
                        fuelToBurn = inventoryController.GetItemModelFromId(fuelSlot.ObjectsAttach[0].ItemType) as FuelItem;
                    }
                }

                ConsumePollutant(fuelToBurn);
                OnProcessFuel?.Invoke(fuelToBurn);
                
                return true;
            }
        }

        return false;
    }

    private void StopFill()
    {
        progressFillProcess.fillAmount = 0;
    }

    private void ConsumePollutant(ItemModel itemFrom)
    {
        if (itemFrom == null)
        {
            return;
        }
        
        if(itemFrom.isPollutant)
        {
            energyHandler.SetValueSecondBar = itemFrom.pollutantDecreaseMaxEnergy;
            int newMaxEnergy = energyHandler.SetMaxEnergy - energyHandler.SetValueSecondBar;
            Debug.Log("Ahora el maximo de la barra de energia queda en: " + newMaxEnergy);
            Debug.Log("Cant energy es: " + energyHandler.cantEnergy);
            ActivateSecondBar?.Invoke();
            if (energyHandler.cantEnergy > newMaxEnergy)
            {
                energyHandler.UpdateEnergy(newMaxEnergy);
            }
        }
    }
    #endregion
}
