using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class MachineSlotView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Image slotFrame = null;
        [SerializeField] private Image iconItemAttach = null;
        [SerializeField] private BoxCollider2D colliderSprite = null;
        [SerializeField] private bool blockItemsInside = false;
        [SerializeField] private TextMeshProUGUI amountOutStack = null;
        [SerializeField] private TextMeshProUGUI debugGridPos = null;
        [SerializeField] private StackSlotHandler stackHandler = null;
        [SerializeField] private Canvas mainCanvas = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<ItemView> objectsAttach = new List<ItemView>();
        private List<ItemView> auxObjectsAttach = new List<ItemView>();

        private Action<Vector2Int, Vector2Int> callUpdateSlots = null;
        private Action<Vector2Int, Vector2Int> callUpdateStacks = null;

        private List<ItemType> allowedItems = new List<ItemType>();
        #endregion

        #region PROPERTIES
        public Vector2 SlotPosition { get { return transform.position; } set { transform.position = value; } }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            stackHandler.Init(mainCanvas, null, this, callUpdateStacks);
        }
        #endregion

        #region PUBLIC_METHODS
        public void AddItemToSlot(ItemView itemToAttach)
        {
            if (blockItemsInside)
            {
                itemToAttach.SwitchStateItem(true);
            }

            if (objectsAttach.Count > 1)
            {
                if (itemToAttach.ItemType == objectsAttach[0].ItemType)
                {
                    itemToAttach.PlaceInMachineSlot(SlotPosition, transform, allowedItems.ToArray());
                }
                else
                {
                    itemToAttach.PlaceInMachineSlot(itemToAttach.SlotPositionAttached.Item1, itemToAttach.SlotPositionAttached.Item3, allowedItems.ToArray());
                }
            }
            else
            {
                itemToAttach.PlaceInMachineSlot(SlotPosition, transform, allowedItems.ToArray());
            }

            ViewAddToSlot(itemToAttach);
        }

        public void PlaceStackOfItems(StackSlotHandler stackComing)
        {
            Debug.Log("Swipe de stacks");

            List<ItemView> stackOfItems = new List<ItemView>();
            stackOfItems.AddRange(stackComing.Stack);

            stackComing.ClearStackOfItems();

            stackHandler.AddItemsOnStack(stackOfItems);
        }

        public void RemoveItemFromSlot(ItemView item)
        {
            if (objectsAttach.Count < 1)
            {
                amountOutStack.text = string.Empty;
                return;
            }

            objectsAttach.Remove(item);

            amountOutStack.text = objectsAttach.Count > 0 ? objectsAttach.Count.ToString() : string.Empty;
        }

        public void RemoveStackFromSlot()
        {

        }
        #endregion

        #region PRIVATE_METHODS
        private void ViewAddToSlot(ItemView item)
        {
            if (!objectsAttach.Contains(item))
            {
                objectsAttach.Add(item);
            }

            amountOutStack.text = objectsAttach.Count.ToString();
        }
        #endregion

        #region GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, colliderSprite.size);
        }
        #endregion
    }
}