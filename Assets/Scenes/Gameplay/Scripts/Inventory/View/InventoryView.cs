using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class InventoryView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("REFERENCES")]
        [SerializeField] private SlotInventoryView prefabSlots = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private Canvas mainCanvas = null;
        [SerializeField] private GameObject prefabItemView = null;
        #endregion

        #region PRIVATE_FIELDS
        private Transform parentView = null;

        private SlotInventoryView[,] slotsView = null;

        private int maxRowsInventory = 0;
        private int maxColsInventory = 0;
        #endregion

        #region PROPERTIES
        public bool IsOpen { set; get; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(InventoryModel model, Transform parentTarget)
        {
            parentView = parentTarget;
            maxRowsInventory = model.GridRows;
            maxColsInventory = model.GridCols;

            InitializeSlotsView(maxRowsInventory, maxColsInventory);

            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    Vector2 finalWorldPosition = new Vector2((parentView.position.x - (model.GridCols * 0.5f)), 
                        (parentView.position.y + (model.GridRows * 0.5f))) + model.GetSlot(gridPos).SlotPosition;

                    SlotInventoryView newSlotInv = Instantiate(prefabSlots, finalWorldPosition, Quaternion.identity, parentView);
                    newSlotInv.Init(prefabItemView, mainCanvas);
                    
                    model.SetSlotPosition(gridPos, finalWorldPosition);
                    Vector2 nextSlotPosition = finalWorldPosition + model.GetSlot(gridPos).NextSlotPosition;
                    model.GetSlot(gridPos).SetPositionNextSlot(nextSlotPosition);

                    slotsView[x, y] = newSlotInv;
                }
            }

            CheckNextSlotsFromSlots(model);

            IsOpen = false;
        }

        public void UpdateSlotsView()
        {
            if (!IsOpen)
                return;

            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);

                    GetSlotFromGrid(gridPos).UpdateSlot();
                }
            }
        }

        public void OpenInventory()
        {
            if (animator == null) return;

            IsOpen = !IsOpen;
            animator.SetBool("IsOpen", IsOpen);
        }

        public SlotInventoryView GetSlotFromGrid(Vector2Int gridPos)
        {
            return IsValidPosition(gridPos) ? slotsView[gridPos.x, gridPos.y] : null;
        }

        public void UpdateInventoryView(InventoryModel inventoryModel)
        {
            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);

                    GetSlotFromGrid(gridPos).UpdateSlotViewWithItems(inventoryModel.GetSlot(gridPos).StackOfItems);
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private bool IsValidPosition(Vector2Int pos)
        {
            return (pos.x < maxRowsInventory && pos.x >= 0 &&
                pos.y < maxColsInventory && pos.y >= 0) ? true : false;
        }
        private void CheckNextSlotsFromSlots(InventoryModel model)
        {
            for (int x = 0; x < model.GridRows; x++)
            {
                for (int y = 0; y < model.GridCols; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    Vector2Int nextPosGrid = new Vector2Int(gridPos.x, gridPos.y + 1);

                    if(GetSlotFromGrid(nextPosGrid) != null)
                    {
                        GetSlotFromGrid(gridPos).NextSlotFromThis = GetSlotFromGrid(nextPosGrid).transform;
                    }
                    else
                    {
                        nextPosGrid = new Vector2Int(gridPos.x + 1, 0);

                        if(GetSlotFromGrid(nextPosGrid) != null)
                        {
                            GetSlotFromGrid(gridPos).NextSlotFromThis = GetSlotFromGrid(nextPosGrid).transform;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private void InitializeSlotsView(int rows, int cols)
        {
            slotsView = new SlotInventoryView[rows, cols];
        }
        #endregion
    }
}