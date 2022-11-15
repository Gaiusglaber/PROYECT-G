using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class UICombinator : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private MachineSlotView inputSlot_Left;
    [SerializeField] private MachineSlotView inputSlot_Right;
    [SerializeField] private MachineSlotView resultSlot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ItemView prefabItemView = null;
    [SerializeField] private GameObject panelCombinator = null;
    [SerializeField] private Canvas mainCanvas = null;
    #endregion

    #region PRIVATE_FIELDS
    private bool isInitialized = false;
    #endregion
}
