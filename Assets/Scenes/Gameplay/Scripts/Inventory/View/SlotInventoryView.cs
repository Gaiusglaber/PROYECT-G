using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;
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
        [SerializeField] private TextMeshProUGUI slotStack = null;
        [SerializeField] private TextMeshProUGUI debugGridPos = null;
        [SerializeField] private StackSlotHandler stackHandler = null;
        [SerializeField] private LayerMask checkOnly = default;
        #endregion

        #region PRIVATE_FIELDS
        private UnityAction atInteract = null;

        private SlotInventoryView nextSlotFromThis = default;

        private GameObject prefabItemView = null;

        private List<ItemView> objectsAttach = new List<ItemView>();

        private Canvas mainCanvas = null;

        private bool attachedStackDone = false;

        private Action<Vector2Int,Vector2Int> callUpdateSlots = null;

        private Vector2Int gridPosition = default;
        #endregion

        #region UNITY_CALLS
        #endregion

        #region PROPERTIES
        public Vector2Int GridPosition { get { return gridPosition; } }

        public List<ItemView> StackOfItemsView { get { return objectsAttach; } }

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
        public void Init(GameObject prefabItemView, Canvas mainCanvas, Vector2Int gridPos, bool debugTxtGrid = false)
        {
            this.mainCanvas = mainCanvas;
            this.prefabItemView = prefabItemView;

            nextSlotFromThis = null;
            slotStack.text = "0";

            stackHandler.Init(mainCanvas, this);

            stackHandler.enabled = false;

            gridPosition = gridPos;

            debugGridPos.text = gridPosition.ToString();
            debugGridPos.gameObject.SetActive(debugTxtGrid);
        }
        public void SetOnSomeItemMoved(Action<Vector2Int, Vector2Int> onSomeItemMoved)
        {
              callUpdateSlots = onSomeItemMoved;
        }

        public void UpdateViewSlot(bool onStackTake)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, colliderSprite.size, 0, transform.forward, 1, checkOnly);

            if(!IsStackUpdated(hits))
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.TryGetComponent(out ItemView newItem))
                    {
                        ViewAddToSlot(newItem);
                    }
                }
            }

            if (onStackTake)
            {
                if (!stackHandler.enabled)
                {
                    stackHandler.enabled=true;
                }

                if (!attachedStackDone)
                {
                    SaveItemsInStack();
                    StartCoroutine(AttachItemsToParent(true, stackHandler.transform));
                    attachedStackDone = true;
                }

                slotStack.text = stackHandler.SizeStack.ToString();
            }
            else
            {
                if(attachedStackDone)
                {
                    if(!stackHandler.Dragged)
                    {
                        if(stackHandler.HasEndedRestoreDrag())
                        {
                            RestoreItemsFromStack();
                            StartCoroutine(AttachItemsToParent(false, transform, () =>
                            {
                                if(stackHandler.enabled)
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

        public void UpdateSlotViewWithItems(List<ItemModel> itemsToStackInSlot)
        {
            UpdateViewWithModelInfo(itemsToStackInSlot);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnInteract?.Invoke();
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
            if(objectsAttach.Count == itemsOnLogicSlot.Count)
            {
                return;
            }
            else
            {
                int amountToRemove = objectsAttach.Count - itemsOnLogicSlot.Count;

                if (amountToRemove < 0)
                {
                    CreateAndAddItemsFromData(itemsOnLogicSlot);
                }
                else
                {
                    RemoveItemsFromDataUpdate(amountToRemove);
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
        private void CreateAndAddItemsFromData(List<ItemModel> itemsOnSlotLogic)
        {
            for (int i = 0; i < itemsOnSlotLogic.Count; i++)
            {
                ItemView newItem = Instantiate(prefabItemView, SlotPosition, Quaternion.identity, transform).GetComponent<ItemView>();
                newItem.GenerateItem(mainCanvas, this, itemsOnSlotLogic[i], callUpdateSlots);

                if (!objectsAttach.Contains(newItem))
                {
                    objectsAttach.Add(newItem);
                }
            }

            slotStack.text = objectsAttach.Count.ToString();
        }

        private void RemoveItemsFromDataUpdate(int diference)
        {
            for (int i = 0; i < diference; i++)
            {
                if(objectsAttach[i] != null)
                {
                    Destroy(objectsAttach[i].gameObject);
                    objectsAttach.Remove(objectsAttach[i]);
                }
            }

            slotStack.text = objectsAttach.Count.ToString();
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
                return true;
            }
            else
            {
                int amountToRemove = objectsAttach.Count - actualItemsInsideSlot.Length;

                if (amountToRemove < 0)
                    return false;

                for (int i = 0; i < amountToRemove; i++)
                {
                    ViewRemoveFromSlot(objectsAttach[objectsAttach.Count - 1]);
                }

                slotStack.text = objectsAttach.Count.ToString();

                return false;
            }
        }

        private void ViewAddToSlot(ItemView item)
        {
            if(!objectsAttach.Contains(item))
            {
                objectsAttach.Add(item);

                slotStack.text = objectsAttach.Count.ToString();
            }
        }

        private void ViewRemoveFromSlot(ItemView item)
        {
            if (objectsAttach.Contains(item))
            {
                objectsAttach.Remove(item);
            }

            if (objectsAttach.Count < 1)
            {
                slotStack.text = string.Empty;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (objectsAttach.Count > 0)
            {
                if(collision.TryGetComponent(out ItemView newItem))
                {
                    if(newItem.ItemType == objectsAttach[0].ItemType)
                    {
                        newItem.AttachToSlot(SlotPosition, GridPosition,transform);
                    }
                    else
                    {
                        if(NextSlotFromThis != null)
                        {
                            if(newItem.AttachToSlot(NextSlotPosition, NextSlotFromThis.GridPosition , NextSlotFromThis.transform))
                            {
                                return;
                            }
                        }
                        else
                        {
                            newItem.AttachToSlot(newItem.SlotPositionAttached.Item1, newItem.SlotPositionAttached.Item2, newItem.SlotPositionAttached.Item3);
                        }
                    }
                }
            }
            else
            {
                if (collision.TryGetComponent(out ItemView item))
                {
                    item.AttachToSlot(SlotPosition, GridPosition ,transform);
                }                
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
            objectsAttach.AddRange(stackHandler.GetStackFormStack());
            slotStack.text = objectsAttach.Count.ToString();
        }
        #endregion
    }
}