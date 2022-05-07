using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.Objects.Inventory.Data
{
    public class SlotInventoryModel
    {
        #region EXPOSED_FIELDS
        #endregion

        #region PRIVATE_FIELDS
        private Vector2Int gridPosition = default;
        private Vector2 position = default;
        private Vector2 nextPosition = default;

        private List<ItemModel> stackOfItemsInSlot = new List<ItemModel>();
        #endregion

        #region PROPERTIES
        public List<ItemModel> StackOfItems { get { return stackOfItemsInSlot; } }
        public Vector2 SlotPosition => position;
        public Vector2 NextSlotPosition => nextPosition;
        public Vector2Int GridPosition => gridPosition;
        #endregion

        #region ACTIONS
        private Action OnItemAttached = null;
        #endregion

        #region PUBLIC_METHODS
        public void SetupSlot(Vector2Int gridPosition, Vector2 position, Vector2 nextPosition ,Action atItemAttach = null)
        {
            this.gridPosition = gridPosition;
            this.position = position;
            this.nextPosition = nextPosition;

            OnItemAttached = atItemAttach;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetPositionNextSlot(Vector2 positionNext)
        {
            nextPosition = positionNext;
        }

        public void PlaceItems(List<ItemModel> itemToAttach) //Esto despues seria x item clase que tendria despues su icono y model x
        {
            if (itemToAttach == null) return;

            stackOfItemsInSlot.AddRange(itemToAttach);

            OnItemAttached?.Invoke();
        }
        #endregion

        #region PRIVATE_METHODS
        #endregion
    }
}