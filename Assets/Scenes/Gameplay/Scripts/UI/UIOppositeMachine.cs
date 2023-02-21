using System;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.Controller;

public class UIOppositeMachine : BaseView
{
    #region EXPOSED_FIELDS
    [SerializeField] private GameObject holderPanel = null;
    [SerializeField] private MachineSlotView inputSlot;
    [SerializeField] private MachineSlotView outputSlot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Image progressFillProcess = null;
    [SerializeField] private EnergyHandler energyHandler = null;
    
    [Header("Item View")]
    [SerializeField] private ItemView prefabItemView = null;
    #endregion

    #region PRIVATE_FIELDS
    private float durationProcess = 0f;
    #endregion

    #region ACTIONS
    public Action ActivateSecondBar = null;
    public Action<ItemModel> OnProcessItem;
    private Func<bool> isOppositeProcessing = null;
    #endregion

    #region PROPERTIES
    public Func<bool> IsOppositeProcessing { set { isOppositeProcessing = value; } get { return isOppositeProcessing; } }
    #endregion

    #region UNITY_CALLS
    void Start()
    {
        inputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);
        outputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);

        inventoryController.OnInteractionChange += inputSlot.SetOnInteractionInventoryChange;
        inventoryController.OnInteractionChange += outputSlot.SetOnInteractionInventoryChange;
    }

    void Update()
    {
        ProcessMaterials();
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetDurationProcess(float timeToBurn)
    {
        durationProcess = timeToBurn;
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
    
    public void UpdateProgressFill(float actualTime)
    {
        float fillValue = (actualTime * 100f) / durationProcess;
        fillValue = fillValue / 100f;

        progressFillProcess.fillAmount = Mathf.MoveTowards(progressFillProcess.fillAmount, fillValue, Time.deltaTime);
    }
    
    public void GenerateOppositeItem(ItemModel itemFrom, Action<bool> canFinishSequence)
    {
        ItemModel finalItem = itemFrom.oppositeItems[0];
        
        ConsumePollutant(itemFrom);

        if(outputSlot == null)
        {
            Debug.LogWarning("Failed to generate result for the processed item, the output slot is NULL");
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
    #endregion
    
    #region PRIVATE_METHODS
    public void OnEndProcess()
    {
        progressFillProcess.fillAmount = 0;
    }
    
    private void ProcessMaterials()
    {
        if (isOppositeProcessing == null)
        {
            return;
        }
        
        if (!IsOppositeProcessing.Invoke())
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
                    OnProcessItem?.Invoke(firstItem);
                }
            }
        }
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

    #region OVERRIDES
    public override void Init()
    {
        base.Init();
    }
    
    public override void TogglePanel()
    {
        holderPanel.SetActive(!holderPanel.activeSelf);
        inventoryController.ToggleInventory();
    }
    #endregion
}
