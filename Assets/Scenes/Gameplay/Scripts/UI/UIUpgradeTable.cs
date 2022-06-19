using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;
public class UIUpgradeTable : MonoBehaviour
{
    [SerializeField] private SlotInventoryView slot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private GameObject prefabItemView = null;
    [SerializeField] private GameObject panelUpgradeTable;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Image progressFillProcess = null;

    private bool extraPositionsCreated = false;

    private List<SlotInventoryView> thisUiSlotsView = new List<SlotInventoryView>();

    private Vector2Int invalidPosition = new Vector2Int(-1, -1);
    public InventoryController InverntoryController { get { return inventoryController; } }

    public Action<bool> UnlockSeparator;

    void Start()
    {
        slot.Init(prefabItemView, mainCanvas, invalidPosition, false);

        thisUiSlotsView.Add(slot);
    }

    void Update()
    {
        CheckItemForUpgrade();
    }

    public void TogglePanel()
    {
        panelUpgradeTable.SetActive(!panelUpgradeTable.activeSelf);
        inventoryController.ToggleInventory();

        ThisIsAwfulButNeeded();
    }

    private void CheckItemForUpgrade()
    {
        if (inventoryController.Model.GetSlot(slot.GridPosition) == null)
            return;
        if(inventoryController.Model.GetSlot(slot.GridPosition).StackOfItems.Count >= 10)
        {
            Debug.Log("Desbloquear separador");

            //Llamar al evento de desbloqueo del separador
            UnlockSeparator?.Invoke(true);
        }
    }

    private void ThisIsAwfulButNeeded()
    {
        if (panelUpgradeTable.activeSelf)
        {
            if (extraPositionsCreated)
                return;

            //Ojo esta parte! Esto es muy feo pero tuve que hacerlo asi porque no encontraba forma de resolverlo
            //literalmente estoy "extendiendo" el inventario para poder interactuar con la UI. Eventualmente esto
            //tendriamos que cambiarlo u encontrar algo mas optimo. Btw por ahora me sirve. :)
            Debug.Log("Creado de posiciones extra del inventario");

            //inventoryController.ExtendInventoryWithExtraSlots(80, 81, 80, 83, thisUiSlotsView);    //Puse 80/83 como para que sean slots imposibles de usar realmente
            inventoryController.ExtendInventoryWithExtraSlots(90, 91, 90, 93, thisUiSlotsView);    //Puse 80/83 como para que sean slots imposibles de usar realmente

            slot.SetSlotGridPosition(inventoryController.GetExtraSlotsFromInventory()[0].GridPosition);

            extraPositionsCreated = true;
        }
        else
        {
            if (inventoryController.Model.GetSlot(slot.GridPosition) == null)
                return;


            if (inventoryController.Model.GetSlot(slot.GridPosition).StackOfItems.Count > 0)
                return;

            Debug.Log("Limpiado de posiciones extra del inventario");

            inventoryController.ClearExtraSlotsInventory();

            slot.SetSlotGridPosition(invalidPosition);

            extraPositionsCreated = false;
        }
    }
}
