using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class UIFurnace : MonoBehaviour
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
    #endregion

    #region PRIVATE_FIELDS
    private Func<bool> isFurnaceProcessing = null;
    private Func<bool> isFurnaceBurningFuel = null;

    private float durationProcess = 0f;

    private List<Vector2Int> savedSlotPositons = new List<Vector2Int>();

    private bool extraPositionsCreated = false;

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
        inputSlot.Init(mainCanvas);
        outputSlot.Init(mainCanvas);
        fuelSlot.Init(mainCanvas, ItemType.fuel);

        extraPositionsCreated = false;

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

    public void GenerateProcessedItem(ItemModel itemFrom)
    {
        ItemModel finalItem = itemFrom.itemResults[0];

        if(outputSlot == null)
        {
            Debug.LogWarning("Failed to generat result for the procesed item, the output slot is NULL");
            return;
        }

        ItemView newItem = Instantiate(prefabItemView, outputSlot.SlotPosition, Quaternion.identity, outputSlot.transform);
        outputSlot.AddItemToSlot(newItem);

        /*if (outputSlot.GridPosition != invalidPosition)
        {
            inventoryController.GenerateItem(finalItem.itemId, outputSlot.GridPosition);
        }
        else
        {
            inventoryController.GenerateItem(finalItem.itemId, savedSlotPositons[1]);
        }*/
    }

    public void TogglePanel()
    {
        panelFurnace.SetActive(!panelFurnace.activeSelf);
        inventoryController.ToggleInventory();

        //ThisIsAwfulButNeeded();        
    }

    public void OnEndProcess()
    {
        progressFillProcess.fillAmount = 0;

        ItemView itemToRemove = null;

        if (inventoryController.StackTake)
        {
            itemToRemove = inputSlot.StackOfItems.Stack[0];
        }
        else
        {
            itemToRemove = inputSlot.ObjectsAttach[0];
        }

        inputSlot.RemoveItemFromSlot(itemToRemove);

        /*if(inputSlot.GridPosition != invalidPosition)
        {
            inventoryController.RemoveItems(inputSlot.GridPosition,1);
        }
        else
        {
            inventoryController.RemoveItems(savedSlotPositons[0], 1);
        }

        inputSlot.UpdateTextOutStack();*/
    }

    public void OnEndBurnOfFuel()
    {
        progressFuelConsumption.fillAmount = 0;

        /*if(fuelSlot.GridPosition != invalidPosition)
        {
            inventoryController.RemoveItems(fuelSlot.GridPosition, 1);
        }
        else
        {
            inventoryController.RemoveItems(savedSlotPositons[2], 1);
        }

        fuelSlot.UpdateTextOutStack();*/
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
            /*if (inventoryController.Model.GetSlot(inputSlot.GridPosition) != null && inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems.Count > 0)
            {
                if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0].itemResults.Count < 1)
                    return;
                
                Debug.Log("Item send to procees");

                ItemModel firstItem = inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0];

                OnProcessMaterial?.Invoke(firstItem);
            }
            else if(savedSlotPositons.Count > 0 && inventoryController.Model.GetSlot(savedSlotPositons[0]) != null && inventoryController.Model.GetSlot(savedSlotPositons[0]).StackOfItems.Count > 0)
            {
                if (inventoryController.Model.GetSlot(savedSlotPositons[0]).StackOfItems[0].itemResults.Count < 1)
                    return;

                Debug.Log("Item send to procees");

                ItemModel firstItem = inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0];

                OnProcessMaterial?.Invoke(firstItem);
            }*/
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

                OnProcessFuel?.Invoke(fuelToBurn);

                return true;
            }

            /*if(inventoryController.Model.GetSlot(fuelSlot.GridPosition) != null && inventoryController.Model.GetSlot(fuelSlot.GridPosition).StackOfItems.Count > 0)
            {
                Debug.Log("Fuel send to burn");
                //fuelSlot.StackHandler.SwitchStateItem(true);

                FuelItem fuelBurning = inventoryController.Model.GetSlot(fuelSlot.GridPosition).StackOfItems[0] as FuelItem;

                OnProcessFuel?.Invoke(fuelBurning);

                return true;
            }
            else if(savedSlotPositons.Count > 0 && inventoryController.Model.GetSlot(savedSlotPositons[2]) != null && inventoryController.Model.GetSlot(savedSlotPositons[2]).StackOfItems.Count > 0)
            {
                Debug.Log("Fuel send to burn");
                //fuelSlot.StackHandler.SwitchStateItem(true);

                FuelItem fuelBurning = inventoryController.Model.GetSlot(fuelSlot.GridPosition).StackOfItems[0] as FuelItem;

                OnProcessFuel?.Invoke(fuelBurning);

                return true;
            }*/
        }

        return false;
    }

    private void StopFill()
    {
        progressFillProcess.fillAmount = 0;
    }

    /*private void ThisIsAwfulButNeeded()
    {
        if (panelFurnace.activeSelf)
        {
            if (extraPositionsCreated)
                return;

            //Ojo esta parte! Esto es muy feo pero tuve que hacerlo asi porque no encontraba forma de resolverlo
            //literalmente estoy "extendiendo" el inventario para poder interactuar con la UI. Eventualmente esto
            //tendriamos que cambiarlo u encontrar algo mas optimo. Btw por ahora me sirve. :)
            Debug.Log("Creado de posiciones extra del inventario");

            inventoryController.ExtendInventoryWithExtraSlots(80, 81, 80, 83, thisUiSlotsView, ref savedSlotPositons);    //Puse 80/83 como para que sean slots imposibles de usar realmente

            inputSlot.SetSlotGridPosition(inventoryController.GetSlotFromGridPosition(savedSlotPositons[0]).GridPosition);
            outputSlot.SetSlotGridPosition(inventoryController.GetSlotFromGridPosition(savedSlotPositons[1]).GridPosition);
            fuelSlot.SetSlotGridPosition(inventoryController.GetSlotFromGridPosition(savedSlotPositons[2]).GridPosition);

            Debug.Log("extra slots del horno 1: " + inventoryController.GetSlotFromGridPosition(savedSlotPositons[0]));
            Debug.Log("extra slots del horno 2: " + inventoryController.GetSlotFromGridPosition(savedSlotPositons[1]));
            Debug.Log("extra slots del horno 3: " + inventoryController.GetSlotFromGridPosition(savedSlotPositons[2]));

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

            inputSlot.SetSlotGridPosition(invalidPosition);
            outputSlot.SetSlotGridPosition(invalidPosition);
            fuelSlot.SetSlotGridPosition(invalidPosition);

            extraPositionsCreated = false;
        }
    }*/
    #endregion
}
