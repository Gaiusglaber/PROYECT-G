using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.Controller;

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

        [Header("TESTING")]
        [SerializeField] private InventoryController inventoryController = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<ItemView> objectsAttach = new List<ItemView>();
        private List<ItemView> auxObjectsAttach = new List<ItemView>();

        private Action<Vector2Int, Vector2Int> callUpdateSlots = null;
        private Action<Vector2Int, Vector2Int> callUpdateStacks = null;
        
        private bool attachedStackDone = false;

        private List<ItemType> allowedItems = new List<ItemType>();
        #endregion

        #region PROPERTIES
        public Vector2 SlotPosition { get { return transform.position; } set { transform.position = value; } }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            stackHandler.Init(mainCanvas, null, this, callUpdateStacks);

            if (inventoryController == null)
            {
                return;
            }

            inventoryController.OnInteractionChange += SetOnInteractionInventoryChange;
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

        public void SetOnInteractionInventoryChange(bool stackIntraction)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (stackIntraction)
            {
                if (!stackHandler.enabled)
                {
                    stackHandler.enabled = true;
                }

                if (!attachedStackDone)
                {
                    SaveItemsInStack();
                    StartCoroutine(AttachItemsToParent(true, stackHandler.transform, () =>
                    {
                        objectsAttach.Clear();
                    }));
                    attachedStackDone = true;
                }

                amountOutStack.text = string.Empty;
            }
            else
            {
                if (attachedStackDone)
                {
                    if (!stackHandler.Dragged)
                    {
                        if (stackHandler.HasEndedRestoreDrag())
                        {
                            RestoreItemsFromStack();
                            StartCoroutine(AttachItemsToParent(false, transform, () =>
                            {
                                if (stackHandler.enabled)
                                {
                                    stackHandler.enabled = false;
                                }
                            }));
                            attachedStackDone = false;
                        }
                    }
                }
            }
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
        private void SaveItemsInStack()
        {
            stackHandler.StackItemsInside(objectsAttach);
        }

        private void RestoreItemsFromStack()
        {
            List<ItemView> allItemsFromStack = stackHandler.GetStackFormStack();

            for (int i = 0; i < allItemsFromStack.Count; i++)
            {
                if (!objectsAttach.Contains(allItemsFromStack[i]))
                {
                    AddItemToSlot(allItemsFromStack[i]);
                }
                else
                {
                    amountOutStack.text = objectsAttach.Count.ToString();
                }
            }
        }

        private void ViewAddToSlot(ItemView item)
        {
            if (!objectsAttach.Contains(item))
            {
                objectsAttach.Add(item);
            }

            amountOutStack.text = objectsAttach.Count.ToString();
        }
        #endregion

        #region CORUTINES
        private IEnumerator AttachItemsToParent(bool stateItemsInside, Transform newParent, Action onEndedAttch = null)
        {
            for (int i = 0; i < objectsAttach.Count; i++)
            {
                if (objectsAttach[i] != null)
                {
                    objectsAttach[i].transform.SetParent(newParent);
                    objectsAttach[i].SwitchStateItem(stateItemsInside);

                    objectsAttach[i].SwitchStateCollider(!stateItemsInside);
                }
            }

            bool allWithNewParent = true;

            for (int i = 0; i < objectsAttach.Count; i++)
            {
                if (objectsAttach[i].transform.parent != newParent)
                {
                    allWithNewParent = false;
                }
            }

            if (allWithNewParent)
            {
                onEndedAttch?.Invoke();
            }

            yield break;
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