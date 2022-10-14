using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Objects.Inventory.Data;

using TMPro;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class SlotInventoryView : MonoBehaviour, IPointerDownHandler
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Image slotFrame = null;
        [SerializeField] private Image iconItemAttach = null;
        [SerializeField] private BoxCollider2D colliderSprite = null;
        [SerializeField] private TextMeshProUGUI amountOutStack = null;
        [SerializeField] private TextMeshProUGUI debugGridPos = null;
        [SerializeField] private StackSlotHandler stackHandler = null;
        [SerializeField] private bool blockItemsInside = false;
        [SerializeField] private LayerMask checkOnly = default;
        #endregion

        #region PRIVATE_FIELDS
        private UnityAction atInteract = null;

        private SlotInventoryView nextSlotFromThis = default;

        private GameObject prefabItemView = null;

        private List<ItemView> objectsAttach = new List<ItemView>();
        private List<ItemView> auxObjectsAttach = new List<ItemView>();

        private Canvas mainCanvas = null;

        private bool attachedStackDone = false;

        private bool stackUpdated = false;

        private Action<Vector2Int, int, bool> onRemoveFromSlot = null;
        private Action<string, int, Vector2Int> onAddedToSlot = null;

        private Action<Vector2Int,Vector2Int> callUpdateSlots = null;
        private Action<Vector2Int,Vector2Int> callUpdateStacks = null;

        private Vector2Int gridPosition = default;

        private List<ItemType> allowedItems = new List<ItemType>();
        #endregion

        #region PROPERTIES
        public string AmountOutStack { get { return amountOutStack.text; } set { amountOutStack.text = value; } }
        
        public StackSlotHandler StackHandler { get { return stackHandler; } set { stackHandler = value; } }

        public Vector2Int GridPosition { get { return gridPosition; } }

        public List<ItemView> StackOfItemsView { get { return objectsAttach; } }

        public List<ItemView> AuxStackOfItems { get { return auxObjectsAttach; } set { auxObjectsAttach = value; } }

        public bool StackUpdated { get { return stackUpdated; } }

        public UnityAction OnInteract
        {
            get
            {
                return atInteract;
            }
            set
            {
                atInteract = value;
            }
        }
        
        public Vector2 SlotPosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Vector2 NextSlotPosition 
        {
            get
            {
                if (NextSlotFromThis != null)
                    return NextSlotFromThis.transform.position;
                else
                    return SlotPosition;
            }            
        }

        public SlotInventoryView NextSlotFromThis
        {
            get
            {
                return nextSlotFromThis;
            }
            set
            {
                nextSlotFromThis = value;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(GameObject prefabItemView, Canvas mainCanvas, Vector2Int gridPos, bool debugTxtGrid = false, params ItemType[] allowedItems)
        {
            this.mainCanvas = mainCanvas;
            this.prefabItemView = prefabItemView;

            nextSlotFromThis = null;
            amountOutStack.text = string.Empty;

            gridPosition = gridPos;

            debugGridPos.text = gridPosition.ToString();
            debugGridPos.gameObject.SetActive(debugTxtGrid);

            this.allowedItems.AddRange(allowedItems);
        }

        public void SetOnSomeItemMoved(Action<Vector2Int, Vector2Int> onSomeItemMoved)
        {
            callUpdateSlots = onSomeItemMoved;
        }

        public void SetOnSomeItemRemoved(Action<Vector2Int, int, bool> onSomeItemRemoved)
        {
            onRemoveFromSlot = onSomeItemRemoved;
        }

        public void SetOnSomeItemAdded(Action<string, int, Vector2Int> onSomeItemAdded)
        {
            onAddedToSlot = onSomeItemAdded;
        }

        public void SetOnSomeStackMoved(Action<Vector2Int, Vector2Int> onSomeStackMoved)
        {
            callUpdateStacks = onSomeStackMoved;

            stackHandler.Init(mainCanvas, this, null,callUpdateStacks, onRemoveFromSlot, onAddedToSlot);
            stackHandler.enabled = false;
        }

        public void SetOnInteractionInventoryChange(bool stackIntraction)
        {
            if (!gameObject.activeInHierarchy || stackHandler.isMachineStack)
                return;

            if (stackIntraction)
            {
                if (!stackHandler.enabled)
                {
                    stackHandler.enabled = true;
                }

                if (!attachedStackDone)
                {
                    if(!stackHandler.isMachineStack)
                    {
                        SaveItemsInStack();
                        StartCoroutine(AttachItemsToParent(true, stackHandler.transform, ()=> 
                        {
                            objectsAttach.Clear();
                        }));
                        attachedStackDone = true;

                        Debug.Log("CHANGE INTERACTION TYPE == STACK: SLOT INVENTORY PA");
                    }
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
                            if (!stackHandler.isMachineStack)
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

                    Debug.Log("CHANGE INTERACTION TYPE == NO STACK: SLOT INVENTORY PA");
                }
            }
        }

        public void SetSlotGridPosition(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;

            debugGridPos.text = gridPosition.ToString();
        }

        public void UpdateSlotViewWithItems(List<ItemModel> itemsToStackInSlot)
        {
            UpdateViewWithModelInfo(itemsToStackInSlot);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnInteract?.Invoke();
        }

        public void UpdateTextOutStack()
        {
            amountOutStack.text = objectsAttach.Count.ToString();
        }

        public void SwipeStacks(StackSlotHandler stackIncoming)
        {
            Debug.Log("Swipe de stacks");

            bool areStackItemsTheSame = false;

            if(stackHandler.Stack.Count > 0 && stackIncoming.Stack.Count > 0)
            {
                if(stackHandler.Stack[0].ItemType == stackIncoming.Stack[0].ItemType)
                {
                    areStackItemsTheSame = true;
                }
            }

            SlotInventoryView theSlotOfTheIncomingStack = stackIncoming.ActualSlot;
            StackSlotHandler auxStack = stackHandler;

            stackHandler = null;

            stackIncoming.SwipeStackSlots(this);
            auxStack.SwipeStackSlots(theSlotOfTheIncomingStack);

            auxStack.ActualSlot = theSlotOfTheIncomingStack;
            stackIncoming.ActualSlot = this;

            stackHandler = stackIncoming;
            theSlotOfTheIncomingStack.stackHandler = auxStack;

            if (areStackItemsTheSame)
            {
                stackHandler.AddItemsOnStack(theSlotOfTheIncomingStack.stackHandler.Stack);
                theSlotOfTheIncomingStack.stackHandler.ClearStackOfItems();
            }

            callUpdateStacks?.Invoke(theSlotOfTheIncomingStack.GridPosition, gridPosition); 
        }

        public void PlaceStackOfItems(StackSlotHandler stackComing)
        {
            Debug.Log("Swipe de stacks");

            List<ItemView> stackOfItems = new List<ItemView>();
            stackOfItems.AddRange(stackComing.Stack);

            for (int i = 0; i < stackOfItems.Count; i++)
            {
                if (stackOfItems[i] != null)
                {
                    if (stackOfItems[i].WasAttachedOnMachine)
                    {
                        stackOfItems[i].WasAttachedOnMachine = false;
                    }
                }
            }

            stackComing.ClearStackOfItems();

            stackHandler.AddItemsOnStack(stackOfItems);

            onAddedToSlot?.Invoke(stackOfItems[0].ItemType, stackOfItems.Count, GridPosition);
        }

        public void PlaceItemOnSlotExternal(ItemView itemToAttach)
        {
            if (blockItemsInside)
            {
                itemToAttach.SwitchStateItem(true);
            }

            if (objectsAttach.Count > 1)
            {
                if (itemToAttach.ItemType == objectsAttach[0].ItemType)
                {
                    itemToAttach.PlaceOnSlotAgain(SlotPosition, GridPosition, transform, allowedItems.ToArray());
                }
                else
                {
                    itemToAttach.PlaceOnSlotAgain(itemToAttach.SlotPositionAttached.Item1, itemToAttach.SlotPositionAttached.Item2, itemToAttach.SlotPositionAttached.Item3, allowedItems.ToArray());
                }
            }
            else
            {
                itemToAttach.PlaceOnSlotAgain(SlotPosition, GridPosition, transform, allowedItems.ToArray());
            }

            ViewAddToSlot(itemToAttach);
        }

        public void PlaceItemOnSlotInternal(ItemView itemToAttach, bool swipeItems)
        {
            if(blockItemsInside)
            {
                itemToAttach.SwitchStateItem(true);
            }

            if(objectsAttach.Count > 1)
            {
                if (itemToAttach.ItemType == objectsAttach[0].ItemType)
                {
                    itemToAttach.AttachToSlot(SlotPosition, GridPosition, transform, swipeItems, allowedItems.ToArray());
                }
                else
                {
                    itemToAttach.AttachToSlot(itemToAttach.SlotPositionAttached.Item1, itemToAttach.SlotPositionAttached.Item2, itemToAttach.SlotPositionAttached.Item3, swipeItems, allowedItems.ToArray());
                   
                    /*if (NextSlotFromThis != null)
                    {
                        if (itemToAttach.AttachToSlot(NextSlotPosition, NextSlotFromThis.GridPosition, NextSlotFromThis.transform, generateItem, allowedItems.ToArray()))
                        {
                            return;
                        }
                    }
                    else
                    {
                        itemToAttach.AttachToSlot(itemToAttach.SlotPositionAttached.Item1, itemToAttach.SlotPositionAttached.Item2, itemToAttach.SlotPositionAttached.Item3, generateItem, allowedItems.ToArray());
                    }*/
                }
            }
            else
            {
                itemToAttach.AttachToSlot(SlotPosition, GridPosition, transform, swipeItems, allowedItems.ToArray());
            }

            ViewAddToSlot(itemToAttach);
        }

        public void RemoveItemsFromSlot()
        {
            if (!stackHandler.enabled)
            {
                onRemoveFromSlot?.Invoke(GridPosition, objectsAttach.Count, true);
            }
            else
            {
                onRemoveFromSlot?.Invoke(GridPosition, stackHandler.Stack.Count, true);
            }
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
        #endregion

        #region GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, colliderSprite.size);
        }
        #endregion

        #region PRIVATE_METHODS
        private void UpdateViewWithModelInfo(List<ItemModel> itemsOnLogicSlot)
        {
            if(!stackHandler.enabled)
            {
                if(objectsAttach.Count == itemsOnLogicSlot.Count)
                {
                    return;
                }
                else
                {
                    int amountToRemove = objectsAttach.Count - itemsOnLogicSlot.Count;

                    if (amountToRemove < 0)
                    {
                        CreateAndAddItemsFromDataOnSlot(itemsOnLogicSlot[0], Mathf.Abs(amountToRemove));
                    }
                    else
                    {
                        RemoveItemsFromDataUpdateOnSlot(amountToRemove);
                    }
                }
            }
            else
            {
                if(stackHandler.Stack.Count == itemsOnLogicSlot.Count)
                {
                    return;
                }
                else
                {
                    int amountToRemove = stackHandler.Stack.Count - itemsOnLogicSlot.Count;

                    if (amountToRemove < 0)
                    {
                        CreateAndAddItemsFromDataOnStack(itemsOnLogicSlot[0], Mathf.Abs(amountToRemove));
                    }
                    else
                    {
                        RemoveItemsFromDataUpdateOnStack(amountToRemove);
                    }
                }
            }
        }

        #region UPDATE_SLOTVIEW_WITH_MODEL
        /// <summary>
        /// Only used to comunicate view with model, if model changes adding some new item, the view inventory will create this items,
        /// and add them to the correct slot.
        /// 
        /// DO NOT USE THIS METHODS TO HANDLE ITEMS. (This is because it CREATES and REMOVE items,
        /// while the drag and change of inventory needs to be with always the same data.)
        /// </summary>
        /// <param name="itemsOnSlotLogic"></param>
        /// 
        public void CreateAndAddItemsFromDataOnSlot(ItemModel itemsTypeOnSlotLogic, int difference)
        {
            for (int i = 0; i < difference; i++)
            {
                ItemView newItem = Instantiate(prefabItemView, SlotPosition, Quaternion.identity, transform).GetComponent<ItemView>();
                newItem.GenerateItem(mainCanvas, this, itemsTypeOnSlotLogic, callUpdateSlots, onRemoveFromSlot, onAddedToSlot);

                if (!objectsAttach.Contains(newItem))
                {
                    objectsAttach.Add(newItem);
                }
            }

            amountOutStack.text = objectsAttach.Count.ToString();
        }
        private void RemoveItemsFromDataUpdateOnSlot(int diference)
        {
            for (int i = 0; i < diference; i++)
            {
                if (objectsAttach.Count < 1)
                {
                    break;
                }

                if (objectsAttach[0] != null)
                {
                    Destroy(objectsAttach[0].gameObject);
                    objectsAttach.Remove(objectsAttach[0]);
                }
            }

            amountOutStack.text = objectsAttach.Count.ToString();
        }

        public void CreateAndAddItemsFromDataOnStack(ItemModel itemsTypeOnSlotLogic, int difference)
        {
            List<ItemView> allItemsToAdd = new List<ItemView>();

            for (int i = 0; i < difference; i++)
            {
                ItemView newItem = Instantiate(prefabItemView, stackHandler.ActualSlot.SlotPosition, Quaternion.identity, transform).GetComponent<ItemView>();
                newItem.GenerateItem(mainCanvas, this, itemsTypeOnSlotLogic, callUpdateSlots, onRemoveFromSlot, onAddedToSlot);

                if (!allItemsToAdd.Contains(newItem))
                {
                    allItemsToAdd.Add(newItem);
                }
            }

            stackHandler.AddItemsOnStack(allItemsToAdd);
        }

        private void RemoveItemsFromDataUpdateOnStack(int diference)
        {
            for (int i = 0; i < diference; i++)
            {
                if (stackHandler.Stack.Count < 1)
                {
                    break;
                }

                if (stackHandler.Stack[0] != null)
                {
                    Destroy(stackHandler.Stack[0].gameObject);
                    stackHandler.Stack.Remove(stackHandler.Stack[0]);

                    stackHandler.UpdateStackAmount();
                }
            }
        }
        #endregion


        /// <summary>
        /// Used to check if the slot has correct items view, if not will update the items on list.
        /// </summary>
        /// <param name="actualItemsInsideSlot"></param>
        /// <returns></returns>
        private bool IsStackUpdated(RaycastHit2D [] actualItemsInsideSlot)
        {
            if(objectsAttach.Count == actualItemsInsideSlot.Length)   //Sin contar el mismo slot que tiene su collider
            {
                stackUpdated = true;
                return true;
            }
            else
            {
                int amountToRemove = objectsAttach.Count - actualItemsInsideSlot.Length;

                if (amountToRemove < 0)
                {
                    stackUpdated = false;
                    return false;
                }

                for (int i = 0; i < amountToRemove; i++)
                {
                    ViewRemoveFromSlot(objectsAttach[objectsAttach.Count - 1]);
                }

                amountOutStack.text = objectsAttach.Count.ToString();

                stackUpdated = false;
                return false;
            }
        }

        private void ViewAddToSlot(ItemView item)
        {
            if(!objectsAttach.Contains(item))
            {
                objectsAttach.Add(item);
            }
            
            amountOutStack.text = objectsAttach.Count.ToString();
        }

        private void ViewRemoveFromSlot(ItemView item)
        {
            if (objectsAttach.Contains(item))
            {
                objectsAttach.Remove(item);
            }

            if (objectsAttach.Count < 1)
            {
                amountOutStack.text = string.Empty;
            }
        }
        #endregion

        #region CORUTINES
        private IEnumerator AttachItemsToParent(bool stateItemsInside,Transform newParent, Action onEndedAttch = null)
        {
            for (int i = 0; i < objectsAttach.Count; i++)
            {
                if(objectsAttach[i] != null)
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

            if(allWithNewParent)
            {
                onEndedAttch?.Invoke();
            }

            yield break;
        }

        private void SaveItemsInStack()
        {
            stackHandler.StackItemsInside(objectsAttach);
        }

        private void RestoreItemsFromStack()
        {
            List<ItemView> allItemsFromStack = stackHandler.GetStackFormStack();

            for (int i = 0; i < allItemsFromStack.Count; i++)
            {
                if(!objectsAttach.Contains(allItemsFromStack[i]))
                {
                    PlaceItemOnSlotInternal(allItemsFromStack[i], true);
                }
                else
                {
                    amountOutStack.text = objectsAttach.Count.ToString();
                }
            }
        }
        #endregion
    }
}