using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        #endregion

        #region PROPERTIES
        public float OffsetSlots => offsetSlots;
        public int GridRows => maxRowsInventory;
        public int GridCols => maxColsInventory;
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector2Int tamGridSlots, Vector2 slotSize, Action onItemAttach = null)
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

                    gridSlots[x, y].SetupSlot(new Vector2Int(x, y), slotPosition, nextSlotPosition, onItemAttach);
                }
            }
        }

        public SlotInventoryModel GetSlot(Vector2Int gridPosition)
        {
            return IsValidPosition(gridPosition) ? gridSlots[gridPosition.x, gridPosition.y] : null;
        }

        public void SetSlotPosition(Vector2Int slot, Vector2 newPos)
        {
            GetSlot(slot).SetPosition(newPos);
        }

        public void AttachObjectAtSlot(GameObject obj, Vector2Int gridPosition)
        {
            GetSlot(gridPosition).PlaceItem(obj);
        }
        #endregion

        #region PRIVATE_METHODS
        private bool IsValidPosition(Vector2Int pos)
        {
            return (pos.x < maxRowsInventory && pos.x >= 0 && 
                pos.y < maxColsInventory && pos.y >= 0) ? true : false;
        }
        #endregion
    }
}