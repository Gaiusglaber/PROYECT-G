using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;

namespace ProyectG.Gameplay.Objects.Inventory.Controller
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
        [Header("ITEM DATABASE")]
        [SerializeField] private List<ItemModel> allItemsAviable = null;
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

            inventoryModel.SetOnSomeItemAttached(inventoryView.UpdateInventoryView);

            //Testing

            Vector2Int gridPos = new Vector2Int(0, 0);
            List<ItemModel> testItems = new List<ItemModel>() { allItemsAviable[0], allItemsAviable[0] , allItemsAviable[0] , allItemsAviable[0] };
            List<ItemModel> testItems1 = new List<ItemModel>() { allItemsAviable[1], allItemsAviable[1] , allItemsAviable[1] };
            List<ItemModel> testItems2 = new List<ItemModel>() { allItemsAviable[2]};
            
            inventoryModel.AttachItemsToSlot(testItems, gridPos);
            inventoryModel.AttachItemsToSlot(testItems1, new Vector2Int(gridPos.x, gridPos.y+1));
            inventoryModel.AttachItemsToSlot(testItems2, new Vector2Int(gridPos.x, gridPos.y +2));
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

            if (!inventoryView.IsOpen)
                return;

            if(Input.GetKeyDown(KeyCode.LeftShift))
            {

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