using System;
using UnityEngine;
using System.Collections.Generic;

using ProyectG.Gameplay.Objects.Inventory.Data;

using TMPro;

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
        [SerializeField] private TMP_Text stackModeState = null;

        [SerializeField]private List<ItemType> allowedItems = null;

        #endregion

        #region PRIVATE_FIELDS
        private Transform parentView = null;

        private SlotInventoryView[,] slotsView = null;

        private int maxRowsInventory = 0;
        private int maxColsInventory = 0;

        private Action<bool> onHandleInventory = null;
        private Action<Vector2Int,Vector2Int> onSomeItemMoved = null;
        private Action<Vector2Int,Vector2Int> onSomeStackMoved = null;
        #endregion

        #region PROPERTIES
        public bool IsOpen { set; get; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(InventoryModel model, Transform parentTarget, Action<Vector2Int, Vector2Int> onSomeItemMoved, Action<Vector2Int,Vector2Int> onSomeStackMoved)
        {
            parentView = parentTarget;
            maxRowsInventory = model.GridRows;
            maxColsInventory = model.GridCols;

            InitializeSlotsView(maxRowsInventory, maxColsInventory);

            this.onSomeItemMoved = onSomeItemMoved;
            this.onSomeStackMoved = onSomeStackMoved;

            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    Vector2 finalWorldPosition = new Vector2((parentView.position.x - (model.GridCols * 0.5f)),
                        (parentView.position.y + (model.GridRows * 0.5f))) + model.GetSlot(gridPos).SlotPosition;

                    SlotInventoryView newSlotInv = Instantiate(prefabSlots, finalWorldPosition, Quaternion.identity, parentView);
                    newSlotInv.Init(prefabItemView, mainCanvas, gridPos, false, allowedItems.ToArray());
                    newSlotInv.SetOnSomeItemMoved(this.onSomeItemMoved);
                    newSlotInv.SetOnSomeStackMoved(this.onSomeStackMoved);

                    model.SetSlotPosition(gridPos, finalWorldPosition);
                    Vector2 nextSlotPosition = finalWorldPosition + model.GetSlot(gridPos).NextSlotPosition;
                    model.GetSlot(gridPos).SetPositionNextSlot(nextSlotPosition);

                    slotsView[x, y] = newSlotInv;
                }
            }

            CheckNextSlotsFromSlots(model);

            IsOpen = false;
        }

        public void UpdateSlots(bool stackTake)
        {
            SetStateTxtStackInfo(stackTake);
            
            if (!IsOpen)
                return;

            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);

                    GetSlotFromGrid(gridPos).UpdateViewSlot(stackTake);
                }
            }
        }

        public void ToggleInventory()
        {
            if (animator == null) return;

            IsOpen = !IsOpen;
            onHandleInventory?.Invoke(!IsOpen);
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

        public void SetOnHandleInventory(Action<bool> onHandleInventory)
        {
            this.onHandleInventory = onHandleInventory;
        }
        #endregion

        #region PRIVATE_METHODS
        private void SetStateTxtStackInfo(bool stackState)
        {
            stackModeState.text = stackState ? "STACK DRAG: ENABLE" : "STACK DRAG: DISABLE";
        }

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
                        GetSlotFromGrid(gridPos).NextSlotFromThis = GetSlotFromGrid(nextPosGrid);
                    }
                    else
                    {
                        nextPosGrid = new Vector2Int(gridPos.x + 1, 0);

                        if(GetSlotFromGrid(nextPosGrid) != null)
                        {
                            GetSlotFromGrid(gridPos).NextSlotFromThis = GetSlotFromGrid(nextPosGrid);
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