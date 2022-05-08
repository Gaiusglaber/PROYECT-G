using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.View;

namespace ProyectG.Gameplay.Objects.Inventory.Data
{
    public class InventoryModel
    {
        #region EXPOSED_FIELDS

        #endregion

        #region PRIVATE_FIELDS
        private int maxRowsInventory = 0;
        private int maxColsInventory = 0;
        private float offsetSlots = 1.5f;

        private SlotInventoryModel[,] gridSlots = null;

        private Action<InventoryModel> onInventoryChange = null;

        private Func<string, ItemModel> onGetItemModelFormDatabase = null;
        #endregion

        #region PROPERTIES
        public float OffsetSlots => offsetSlots;
        public int GridRows => maxRowsInventory;
        public int GridCols => maxColsInventory;
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector2Int tamGridSlots, Vector2 slotSize)
        {
            maxRowsInventory = tamGridSlots.x;
            maxColsInventory = tamGridSlots.y;

            gridSlots = new SlotInventoryModel[tamGridSlots.x, tamGridSlots.y];

            for (int x = 0;  x < maxRowsInventory;  x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    gridSlots[x, y] = new SlotInventoryModel();

                    Vector2 slotPosition = new Vector2((x * (slotSize.x) + OffsetSlots), (y * (slotSize.y) + OffsetSlots));
                    Vector2 nextSlotPosition = new Vector2(slotPosition.x + (slotSize.x + OffsetSlots), slotPosition.y + (slotSize.y + OffsetSlots));

                    gridSlots[x, y].SetupSlot(new Vector2Int(x, y), slotPosition, nextSlotPosition, AtInventoryChange);
                    gridSlots[x, y].SetOnGetModelItem(GetItemModelFromID);
                }
            }

            CheckNextSlotsFromSlots();
        }

        public void UpdateInventoryModel(InventoryView inventoryView)
        {
            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);

                    GetSlot(gridPos).UpdateSlotViewWithItems(inventoryView.GetSlotFromGrid(gridPos).StackOfItemsView);
                }
            }
        }

        public void SetOnSomeItemAdded(Action<InventoryModel> onItemAttach = null)
        {
            onInventoryChange = onItemAttach;
        }

        public void SetOnGetItemModelFromDatabae(Func<string, ItemModel> onGetItem)
        {
            onGetItemModelFormDatabase = onGetItem;
        }

        public SlotInventoryModel GetSlot(Vector2Int gridPosition)
        {
            return IsValidPosition(gridPosition) ? gridSlots[gridPosition.x, gridPosition.y] : null;
        }

        public void SetSlotPosition(Vector2Int slot, Vector2 newPos)
        {
            GetSlot(slot).SetPosition(newPos);
        }

        //USE THIS TO ATTACH AN ITEM/ITEMS TO THE INVENTORY
        public void AttachItemsToSlot(List<ItemModel> stackOfItems, Vector2Int gridPosition = default)
        {
            GetSlot(gridPosition).PlaceItems(stackOfItems);
        }

        public void DeattachItemsFromSlot(Vector2Int gridPosition, int amount = 0, bool allItems = true)
        {
            GetSlot(gridPosition).RemoveItems(amount, allItems);
        }

        public List<ItemModel> GetItemsOnSlot(Vector2Int gridPosition)
        {
            return GetSlot(gridPosition).StackOfItems;
        }
        #endregion

        #region PRIVATE_METHODS
        private void AtInventoryChange()
        {
            onInventoryChange?.Invoke(this);
        }

        private ItemModel GetItemModelFromID(string idModelItem)
        {
            return onGetItemModelFormDatabase?.Invoke(idModelItem);
        }

        private void CheckNextSlotsFromSlots()
        {
            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    Vector2Int nextPosGrid = new Vector2Int(gridPos.x, gridPos.y + 1);

                    if (GetSlot(nextPosGrid) != null)
                    {
                        GetSlot(gridPos).NextGridSlot = GetSlot(nextPosGrid);
                    }
                    else
                    {
                        nextPosGrid = new Vector2Int(gridPos.x + 1, 0);

                        if (GetSlot(nextPosGrid) != null)
                        {
                            GetSlot(gridPos).NextGridSlot = GetSlot(nextPosGrid);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private bool IsValidPosition(Vector2Int pos)
        {
            return (pos.x < maxRowsInventory && pos.x >= 0 && 
                pos.y < maxColsInventory && pos.y >= 0) ? true : false;
        }
        #endregion
    }
}