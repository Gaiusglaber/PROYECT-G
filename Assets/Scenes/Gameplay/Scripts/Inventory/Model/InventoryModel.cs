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

        private Vector2Int invalidPosition = new Vector2Int(-1,-1);  
        #endregion

        #region PROPERTIES
        public float OffsetSlots => offsetSlots;
        public int GridRows => maxRowsInventory;
        public int GridCols => maxColsInventory;
        public SlotInventoryModel[,] GridSlots { get { return gridSlots; } }
        #endregion

        #region PUBLIC_METHODS

        #region INITIALIATION
        public void Init(Vector2Int tamGridSlots, Vector2 slotSize)
        {
            maxRowsInventory = tamGridSlots.x;
            maxColsInventory = tamGridSlots.y;

            gridSlots = new SlotInventoryModel[tamGridSlots.x, tamGridSlots.y];

            for (int x = 0; x < maxRowsInventory; x++)
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

        public void SetOnSomeItemAdded(Action<InventoryModel> onItemAttach = null)
        {
            onInventoryChange = onItemAttach;
        }

        public void SetOnGetItemModelFromDatabae(Func<string, ItemModel> onGetItem)
        {
            onGetItemModelFormDatabase = onGetItem;
        }

        public void SetSlotPosition(Vector2Int slot, Vector2 newPos)
        {
            GetSlot(slot).SetPosition(newPos);
        }
        #endregion

        #region GETERS
        public SlotInventoryModel GetSlot(Vector2Int gridPosition)
        {
            return IsValidPosition(gridPosition) ? gridSlots[gridPosition.x, gridPosition.y] : null;
        }
        public List<ItemModel> GetItemsOnSlot(Vector2Int gridPosition)
        {
            return GetSlot(gridPosition).StackOfItems;
        }
        #endregion

        #region OTHERS
        //USE THIS TO ATTACH AN ITEM/ITEMS TO THE INVENTORY
        public void AttachItemsToSlot(List<ItemModel> stackOfItems, Vector2Int gridPosition = default)
        {
            if(gridPosition != default)
            {
                Debug.Log("Custom Grid Position");
                GetSlot(gridPosition).PlaceItems(stackOfItems);
                return;
            }

            Vector2Int slotWithThisItem = FindSlotWithItem(stackOfItems[0].itemId);

            if(slotWithThisItem != invalidPosition)
            {
                Debug.Log("Grid Position with this type of item");

                GetSlot(slotWithThisItem).PlaceItems(stackOfItems);
                return;
            }

            Vector2Int emptySlot = FindEmptySlot();

            if(emptySlot != invalidPosition)
            {
                Debug.Log("Empty Grid Position");

                GetSlot(emptySlot).PlaceItems(stackOfItems);
                return;
            }

            Debug.LogWarning("No hay mas espacio en el inventario!");
        }

        public void DeattachItemsFromSlot(Vector2Int gridPosition, int amount = 0, bool allItems = true)
        {
            GetSlot(gridPosition).RemoveItems(amount, allItems);
        }

        /// <summary>
        /// This method is only used when only one item is switched form the inventory view.
        /// Making that if you move one item to other slot the internal data is updated with that information.
        /// 
        /// THIS WILL NOT WORK IF A STACK WAS MOVED (We need to create another method that swicth ITEM(S) from one to another slot)
        /// </summary>
        /// <param name="originalSlot"></param>
        /// <param name="newSlot"></param>
        public void SiwtchItemsOnSlots(Vector2Int originalSlot, Vector2Int newSlot)
        {
            if (originalSlot == newSlot)
            {
                Debug.Log("Mismo slot");
                return;
            }

            //Debug.Log("SWITCHED ITEM FROM {" + originalSlot + "} TO {" + newSlot + "}");

            if(GetSlot(originalSlot) != null)
            {
                if(GetSlot(originalSlot).StackOfItems.Count > 0)
                {
                    if(GetSlot(newSlot) != null)
                    {
                        GetSlot(newSlot).PlaceOneItem(GetSlot(originalSlot).StackOfItems[0]);
                        GetSlot(originalSlot).RemoveItems(1, false, false);
                    }
                    else
                    {
                        GetSlot(originalSlot).RemoveItems(1, false, false);
                    }
                }
            }
            /*else
            {
                ItemModel itemFromOutInventory = new ItemModel();

                GetSlot(newSlot).PlaceOneItem();
            }*/

            //Testing
            for (int x = 0; x < GridRows; x++)
            {
                for (int y = 0; y < GridCols; y++)
                {
                    Vector2Int gridSlot = new Vector2Int(x, y);

                    Debug.Log("SLOT {" + gridSlot + "} has " + GetSlot(gridSlot).StackOfItems.Count + " items");
                }
            }
        }

        /// <summary>
        /// This is the update of the model when you switch in stack mode
        /// </summary>
        /// <param name="fromSlot"></param>
        /// <param name="toSlot"></param>
        public void SiwtchStackOfItemsOnSlots(Vector2Int fromSlot, Vector2Int toSlot)
        {
            if (fromSlot == toSlot)
            {
                Debug.Log("Mismo slot");
                return;
            }

            Debug.Log("SWITCHED STACK OF ITEMS FROM {" + fromSlot + "} TO {" + toSlot + "}");

            List<ItemModel> allFromItems = new List<ItemModel>();
            List<ItemModel> allToItems = new List<ItemModel>();
            allFromItems.AddRange(GetSlot(fromSlot).StackOfItems);
            allToItems.AddRange(GetSlot(toSlot).StackOfItems);

            GetSlot(toSlot).RemoveItems(0, true, false);
            GetSlot(toSlot).PlaceItems(allFromItems, false);

            GetSlot(fromSlot).RemoveItems(0, true, false);
            GetSlot(fromSlot).PlaceItems(allToItems, false);

            //Testing
            for (int x = 0; x < GridRows; x++)
            {
                for (int y = 0; y < GridCols; y++)
                {
                    Vector2Int gridSlot = new Vector2Int(x, y);

                    Debug.Log("SLOT {" + gridSlot + "} has " + GetSlot(gridSlot).StackOfItems.Count + " items");
                }
            }
        }
        #endregion

        #endregion

        #region PRIVATE_METHODS
        private void CopyItemModel(ref ItemModel newItem, ItemModel fromItem)
        {
            newItem.itemId = fromItem.itemId;
            newItem.itemDescription = fromItem.itemDescription;
            newItem.itemSprite = fromItem.itemSprite;
            newItem.itemValue = fromItem.itemValue;
        }

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

        private Vector2Int FindSlotWithItem(string idItem)
        {
            Vector2Int findedSlot = invalidPosition;

            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);

                    if(GetSlot(gridPosition).StackOfItems.Count > 0)
                    {
                        string comparationId = GetSlot(gridPosition).StackOfItems[0].itemId;

                        if(comparationId == idItem)
                        {
                            findedSlot = gridPosition;
                            return findedSlot;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
            }

            return findedSlot;
        }

        private Vector2Int FindEmptySlot()
        {
            Vector2Int findedSlot = invalidPosition;

            for (int x = 0; x < maxRowsInventory; x++)
            {
                for (int y = 0; y < maxColsInventory; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);

                    if(GetSlot(gridPosition).IsEmpty)
                    {
                        findedSlot = gridPosition;
                        return findedSlot;
                    }
                }
            }

            return findedSlot;
        }

        private bool IsValidPosition(Vector2Int pos)
        {
            return (pos.x < maxRowsInventory && pos.x >= 0 && 
                pos.y < maxColsInventory && pos.y >= 0) ? true : false;
        }
        #endregion
    }
}