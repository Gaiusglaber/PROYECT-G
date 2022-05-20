using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.View;

namespace ProyectG.Gameplay.Objects.Inventory.Data
{
    public class SlotInventoryModel
    {
        #region PRIVATE_FIELDS
        private Vector2Int gridPosition = default;
        private SlotInventoryModel nextGridSlot = default;
        private Vector2 position = default;
        private Vector2 nextPosition = default;

        private List<ItemModel> stackOfItemsInSlot = new List<ItemModel>();

        private bool isEmpty = false;

        private Func<string,ItemModel> getItemModelFromId = null;
        #endregion

        #region PROPERTIES
        public List<ItemModel> StackOfItems { get { return stackOfItemsInSlot; } }
        public Vector2 SlotPosition => position;
        public Vector2 NextSlotPosition => nextPosition;
        public Vector2Int GridPosition => gridPosition;
        public SlotInventoryModel NextGridSlot { get { return nextGridSlot; } set { nextGridSlot = value; } }
        public bool IsEmpty => isEmpty;
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

            isEmpty = true;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetPositionNextSlot(Vector2 positionNext)
        {
            nextPosition = positionNext;
        }

        public void SetNextSlotGridModel(SlotInventoryModel nextSlotFromThis)
        {
            nextGridSlot = nextSlotFromThis;
        }

        public void SetOnGetModelItem(Func<string, ItemModel> onGetItemModel)
        {
            getItemModelFromId = onGetItemModel;
        }

        public void UpdateSlotViewWithItems(List<ItemView> itemsToStackInSlot)
        {
            if(!IsStackUpdated(itemsToStackInSlot, out int difference))
            {
                if(difference < 0)
                {
                    difference = Mathf.Abs(difference);
                    RemoveItems(difference);
                }
                else
                {
                    List<ItemModel> newListOfItems = new List<ItemModel>();

                    //Para crear los items extras añadidos a este slot, busco el ID de model que nesecito, llamando un func que te lo busca
                    //en la database del inventoryController y te lo retorna.
                    ItemModel modelType = getItemModelFromId?.Invoke(itemsToStackInSlot[0].ItemType);  

                    for (int i = 0; i < stackOfItemsInSlot.Count + difference; i++)
                    {
                        newListOfItems.Add(modelType);
                    }

                    PlaceItems(newListOfItems);
                }
            }

            Debug.Log("SLOT {"+ GridPosition+"} has " + stackOfItemsInSlot.Count + " items inside");
        }

        public void RemoveItems(int amount = 0, bool allItems = true, bool updateView = true)
        {
            if (amount != 0)
                allItems = false;

            if(allItems)
            {
                stackOfItemsInSlot.Clear();
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    if (stackOfItemsInSlot.Count < 1)
                        break;

                    stackOfItemsInSlot.RemoveAt(0);
                }
            }

            if(updateView)
            {
                OnItemAttached?.Invoke();
            }
        }

        public void PlaceOneItem(ItemModel itemToAttach)
        {
            if (itemToAttach == null)
                return;

            if (stackOfItemsInSlot.Count < 1)
            {
                isEmpty = true;
            }

            if(isEmpty)
            {
                stackOfItemsInSlot.Add(itemToAttach);
                isEmpty = false;
            }
            else
            {
                if(stackOfItemsInSlot[0] != null)
                {
                    if (AreItemsEquals(itemToAttach.itemId, stackOfItemsInSlot[0].itemId))
                    {
                        stackOfItemsInSlot.Add(itemToAttach);
                    }
                    else
                    {
                        if(NextGridSlot != null)
                        {
                            NextGridSlot.PlaceOneItem(itemToAttach);
                        }
                    }
                }
            }
        }

        public void PlaceItems(List<ItemModel> itemToAttach, bool updateView = true)
        {
            if (itemToAttach == null || itemToAttach.Count < 1) return;

            if(stackOfItemsInSlot.Count < 1)
            {
                isEmpty = true;
            }

            if(isEmpty)
            {
                stackOfItemsInSlot.AddRange(itemToAttach);

                isEmpty = false;

                if(updateView)
                {
                    OnItemAttached?.Invoke();
                }
            }
            else
            {
                if (itemToAttach[0] != null && stackOfItemsInSlot[0] != null)
                {
                    if (AreItemsEquals(itemToAttach[0].itemId, stackOfItemsInSlot[0].itemId))
                    {
                        stackOfItemsInSlot.AddRange(itemToAttach);

                        if (updateView)
                        {
                            OnItemAttached?.Invoke();
                        }
                    }
                    else
                    {
                        if(NextGridSlot != null)
                        {
                            NextGridSlot.PlaceItems(itemToAttach);
                        }
                    }
                }
            }
        }

        public Func<string,ItemModel> GetItemModelFromId()
        {
            return getItemModelFromId;
        }

        #endregion

        #region PRIVATE_METHODS
        private bool AreItemsEquals(string incomingModelID, string someItemInStackIds)
        {
            return incomingModelID == someItemInStackIds;
        }

        private bool IsStackUpdated(List<ItemView> itemsOnViewSlot, out int difference)
        {
            if(stackOfItemsInSlot.Count == itemsOnViewSlot.Count)
            {
                difference = 0;

                return true;
            }
            else
            {
                difference = itemsOnViewSlot.Count - stackOfItemsInSlot.Count;

                return false;
            }
        }
        #endregion
    }
}