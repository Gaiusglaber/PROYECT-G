using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;

namespace ProyectG.Gameplay.Objects.Inventory
{
    public class InventoryController : MonoBehaviour   //Controlador del inventario
    {
        #region EXPOSED_FIELDS
        [Header("MAIN SETTINGS")]
        [SerializeField, Tooltip("This is a grid size, so the amount of slots will be rows per column [ Note->(x * y) ].")] 
        private Vector2Int bagSlots = default;
        [SerializeField, Tooltip("The logic spacing between slots.")] private Vector2 slotsSize = default;
        [Header("REFERENCES")]
        [SerializeField] private Transform viewParent = default;
        [SerializeField] private InventoryView inventoryView = null;
        #endregion

        #region PRIVATE_FIELDS
        private InventoryModel inventoryModel = null;
        #endregion

        public InventoryModel Model => inventoryModel;

        #region PUBLIC_METHODS
        public void Init()
        {
            InitilizeMVC();

            Vector2 initialPos = new Vector2(viewParent.position.x, viewParent.position.y);

            inventoryModel.Init(bagSlots, slotsSize); //data del inventario
            inventoryView.Init(inventoryModel, viewParent); //visual del inventario
        }

        public void UpdateInventory()
        {
            if (inventoryView == null)
                return;

            inventoryView.UpdateSlotsView();
        }

        public void CheckState()
        {
            if (inventoryView == null) return;

            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryView.OpenInventory();
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitilizeMVC()
        {
            inventoryModel = new InventoryModel();
        }
        #endregion
    }
}