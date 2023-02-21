using System;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.Controller;

public class UIOppositeMachine : BaseView
{
    [SerializeField] private GameObject holderPanel = null;
    [SerializeField] private MachineSlotView inputSlot;
    [SerializeField] private MachineSlotView outputSlot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private Canvas mainCanvas = null;
    //[SerializeField] private Image progressFillProcess = null;
    [SerializeField] private EnergyHandler energyHandler = null;

    public Action<ItemModel> OnProcessItem;

    void Start()
    {
        inputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);
        outputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);
    }

    void Update()
    {
    }

    public override void TogglePanel()
    {
        holderPanel.SetActive(!holderPanel.activeSelf);
        inventoryController.ToggleInventory();
    }
}
