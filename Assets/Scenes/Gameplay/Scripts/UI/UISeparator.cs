using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class UISeparator : MonoBehaviour
{
    [SerializeField] private SlotInventoryView inputSlot;
    [SerializeField] private SlotInventoryView outputSlot1;
    [SerializeField] private SlotInventoryView outputSlot2;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private GameObject prefabItemView = null;
    [SerializeField] private GameObject panelSeparator = null;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Image progressFillProcess = null;

    private Func<bool> isSeparatorProcessing = null;

    private float durationProcess = 0.0f;
    private bool extraPositionsCreated = false;

    private List<SlotInventoryView> thisUiSlotsView = new List<SlotInventoryView>();

    private Vector2Int invalidPosition = new Vector2Int(-1, -1);

    public Action<ItemModel> OnProcessMaterial;
    public Action onCancelProcess = null;

    public InventoryController InverntoryController { get { return inventoryController; } }
    public Func<bool> IsSeparatorProcessing { set { isSeparatorProcessing = value; } get { return isSeparatorProcessing; } }

    void Start()
    {
        inputSlot.Init(prefabItemView, mainCanvas, invalidPosition, false);
        outputSlot1.Init(prefabItemView, mainCanvas, invalidPosition, false);
        outputSlot2.Init(prefabItemView, mainCanvas, invalidPosition, false);

        thisUiSlotsView.Add(inputSlot);
        thisUiSlotsView.Add(outputSlot1);
        thisUiSlotsView.Add(outputSlot2);
        extraPositionsCreated = false;
    }

    void Update()
    {
        inputSlot.UpdateViewSlot(inventoryController.StackTake);
        outputSlot1.UpdateViewSlot(inventoryController.StackTake);
        outputSlot2.UpdateViewSlot(inventoryController.StackTake);
    }

    public void SetDurationProcess(float timeToSeparate)
    {
        durationProcess = timeToSeparate;
    }

    public void GenerateProcessedItems(ItemModel itemFrom1, ItemModel itemFrom2)
    {
        ItemModel finalItem = itemFrom1.itemResults[0];
        inventoryController.GenerateItem(finalItem.itemId, outputSlot1.GridPosition);

        ItemModel finalItem2 = itemFrom2.itemResults[0];
        inventoryController.GenerateItem(finalItem2.itemId, outputSlot2.GridPosition);
    }

    public void TogglePanel()
    {
        panelSeparator.SetActive(!panelSeparator.activeSelf);
        inventoryController.ToggleInventory();
    }

    public void OnEndProcess()
    {
        progressFillProcess.fillAmount = 0;

        inventoryController.RemoveItems(inputSlot.GridPosition, 1);

        inputSlot.UpdateTextOutStack();
    }

    public void UpdateProgressFill(float actualTime)
    {
        float fillValue = (actualTime * 100.0f) / durationProcess;
        fillValue = fillValue / 100.0f;

        progressFillProcess.fillAmount = Mathf.MoveTowards(progressFillProcess.fillAmount, fillValue, Time.deltaTime);
    }

    private void ProcessMaterials()
    {
        if (!IsSeparatorProcessing.Invoke())
        {
            if (inventoryController.Model.GetSlot(inputSlot.GridPosition) == null)
                return;

            if(inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems.Count > 0)
            {
                if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0].itemResults.Count < 1)
                    return;

                Debug.Log("Item send to separate");

                ItemModel firstItem = inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0];

                OnProcessMaterial?.Invoke(firstItem);

            }

        }
    }

    private void StopFill()
    {
        progressFillProcess.fillAmount = 0;
    }

}
