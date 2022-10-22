using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;

using ProyectG.Toolbox.Lerpers;

using TMPro;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class StackSlotHandler : MonoBehaviour, IDraggable, ISwitchable, IPointerEnterHandler, IPointerExitHandler
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float speedFollow = 0;
        [SerializeField] private TextMeshProUGUI stackAmount = null;
        [SerializeField] private bool useLerpAnimations = true;
        #endregion

        #region PRIVATE_FIELDS
        private Canvas canvas = null;
        private Camera mainCamera = null;
        private PointerEventData eventData = null;
        private RectTransform draggingPlane;

        private Vector3Lerper positionLerper = null;

        private bool isDragging = false;
        private bool prepareToAttachOnSlot = false;
        private bool isAttachedToSlot = false;
        private bool onRestoreDrag = false;
        private bool onSwipingStack = false;
        private bool dragDisable = false;

        private (Vector2,Vector2Int,Transform) slotPositionAttached = default;
        private (Vector2,Vector2Int,Transform) lastslotPositionAttached = default;

        private Action<Vector2Int, Vector2Int> onEndedChangeOfSlot = null;
        private Action<string, bool> OnHoverSelection;

        private SlotInventoryView actualSlot = null;

        float timeToGoBackSlot = 0.5f;
        float time = 0;

        private BoxCollider2D myCollider = null;
        private RectTransform thisRect = null;

        private List<ItemView> stackedItems = new List<ItemView>();
        #endregion

        #region PROPERTIES
        public bool Dragged => isDragging;
        public int SizeStack => stackedItems.Count;
        public List<ItemView> Stack { get { return stackedItems; } }
        public SlotInventoryView ActualSlot { get { return actualSlot; } set { actualSlot = value; } }
        public (Vector2, Vector2Int, Transform) SlotPositionAttached { get { return slotPositionAttached; } }
        public (Vector2, Vector2Int, Transform) LastslotPositionAttached { get { return lastslotPositionAttached; } }

        #endregion

        #region PUBLIC_METHODS
        public void Init(Canvas mainCanvas, SlotInventoryView slotAttached, Action<Vector2Int,Vector2Int> onStackMoved, Action<string, bool> OnHoverSelection)
        {
            onEndedChangeOfSlot = onStackMoved;

            positionLerper = new Vector3Lerper(speedFollow, Vector3Lerper.SMOOTH_TYPE.STEP_SMOOTHER);

            canvas = mainCanvas;
            mainCamera = Camera.main;

            myCollider = GetComponent<BoxCollider2D>();

            slotPositionAttached.Item1 = slotAttached.SlotPosition;
            slotPositionAttached.Item2 = slotAttached.GridPosition;
            slotPositionAttached.Item3 = slotAttached.transform;

            actualSlot = slotAttached;

            thisRect = transform as RectTransform;

            stackAmount.text = string.Empty;

            if (!isAttachedToSlot)
            {
                AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2, slotPositionAttached.Item3);
                isAttachedToSlot = true;
            }

            this.OnHoverSelection = OnHoverSelection;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (stackedItems.Count < 1)
                return;

            if (dragDisable)
                return;

            isDragging = true;

            prepareToAttachOnSlot = false;

            gameObject.transform.SetParent(canvas.transform, false);
            gameObject.transform.SetAsLastSibling();

            draggingPlane = canvas.transform as RectTransform;

            RectTransform objectDragging = gameObject.transform as RectTransform;

            objectDragging.position = eventData.position;

            this.eventData = eventData;

            SetDraggedPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!Dragged || gameObject == null || stackedItems.Count < 1)
                return;

            if (dragDisable)
                return;

            SetDraggedPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.eventData = eventData;
            isDragging = false;

            prepareToAttachOnSlot = true;
            onRestoreDrag = true;

            draggingPlane = null;

            CheckOverSlot();
        }

        public void AddItemsOnStack(List<ItemView> listOfItems)
        {
            for (int i = 0; i < listOfItems.Count; i++)
            {
                stackedItems.Add(listOfItems[i]);

                listOfItems[i].transform.SetParent(transform);
                listOfItems[i].transform.position = transform.position;

                listOfItems[i].SwitchStateItem(true);
                listOfItems[i].SwitchStateCollider(false);
            }
            
            stackAmount.text = SizeStack > 0 ? SizeStack.ToString() : string.Empty;
        }

        public void ClearStackOfItems()
        {
            stackedItems.Clear();
            stackAmount.text = SizeStack > 0 ? SizeStack.ToString() : string.Empty;
        }

        public void StackItemsInside(List<ItemView> listOfItems)
        {
            stackedItems.Clear();

            for (int i = 0; i < listOfItems.Count; i++)
            {
                stackedItems.Add(listOfItems[i]);
            }

            stackAmount.text = SizeStack > 0 ? SizeStack.ToString() : string.Empty;
        }

        public List<ItemView> GetStackFormStack()   //no supe como llamarlo mejor xd
        {
            List<ItemView> allReturnedItems = new List<ItemView>();

            allReturnedItems.AddRange(stackedItems);
            stackedItems.Clear();

            stackAmount.text = string.Empty;

            return allReturnedItems;
        }

        public void UpdateStackAmount()
        {
            stackAmount.text = SizeStack > 0 ? SizeStack.ToString() : string.Empty;
        }

        public void SwitchStateItem(bool state)
        {
            dragDisable = state;
        }
        #endregion

        #region PRIVATE_METHODS
        private void SetDraggedPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
            {
                draggingPlane = eventData.pointerEnter.transform as RectTransform;
            }

            this.eventData = eventData;
            
            RectTransform objectDragging = gameObject.transform as RectTransform;
            Vector3 globalMouse = default;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMouse))
            {
                objectDragging.position = Vector3.Lerp(objectDragging.position, globalMouse, speedFollow);
                objectDragging.rotation = draggingPlane.rotation;
            }
        }

        private void UpdateItemsInsideSatckAfterMove()
        {
            List<ItemView> itemsCopy = new List<ItemView>();

            itemsCopy.AddRange(stackedItems);

            for (int i = 0; i < itemsCopy.Count; i++)
            {
                if(itemsCopy[i] != null)
                {
                    itemsCopy[i].UpdateItemSlot(SlotPositionAttached);
                }
            }
        }

        private bool CheckOverSlot()
        {
            Ray2D rayFromThisItem = new Ray2D(thisRect.position - (Vector3.forward * 15f), thisRect.forward * 50f);

            RaycastHit2D[] allHits = Physics2D.RaycastAll(rayFromThisItem.origin, rayFromThisItem.direction, 50f);

            foreach (RaycastHit2D hit in allHits)
            {
                if (hit.collider.TryGetComponent(out SlotInventoryView slotFromItem))
                {
                    if (slotFromItem != ActualSlot)
                    { 
                        slotFromItem.SwipeStacks(this);

                        actualSlot = slotFromItem;

                        return true;
                    }
                }
            }

            if (actualSlot != null)
            {
                SwipeStackSlots(actualSlot);
            }

            Debug.DrawRay(rayFromThisItem.origin, rayFromThisItem.direction * 50f, Color.green);

            return false;
        }

        public bool AttachToSlot(Vector2 positionSlot, Vector2Int gridPos,Transform parent, params ItemType[] allowedTypes)
        {
            if (parent == null)
            {
                AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2, slotPositionAttached.Item3);
                return false;
            }

            if (!isDragging)
            {
                lastslotPositionAttached = slotPositionAttached;

                if (prepareToAttachOnSlot)
                {
                    isAttachedToSlot = true;
                    slotPositionAttached.Item1 = positionSlot;
                    slotPositionAttached.Item2 = gridPos;
                    slotPositionAttached.Item3 = parent;
                }

                if (useLerpAnimations)
                {
                    StartCoroutine(AttachToPosition(positionSlot, () =>
                    {
                        onEndedChangeOfSlot?.Invoke(lastslotPositionAttached.Item2, slotPositionAttached.Item2);

                        transform.SetParent(parent);
                        myCollider.enabled = true;
                        onRestoreDrag = false;
                    }));
                }
                else
                {
                    onEndedChangeOfSlot?.Invoke(lastslotPositionAttached.Item2, slotPositionAttached.Item2);

                    transform.position = positionSlot;
                    transform.SetParent(parent);
                    myCollider.enabled = true;
                    onRestoreDrag = false;
                }

                return true;
            }

            return false;
        }

        public bool SwipeStackSlots(SlotInventoryView newSlot, Action callback = null)
        {
            myCollider.enabled = false;
            onSwipingStack = true;

            if(!isDragging)
            {
                slotPositionAttached.Item1 = newSlot.SlotPosition;
                slotPositionAttached.Item2 = newSlot.GridPosition;
                slotPositionAttached.Item3 = newSlot.transform;

                if(useLerpAnimations)
                {
                    StartCoroutine(AttachToPosition(newSlot.SlotPosition, ()=> {

                        transform.SetParent(newSlot.transform);
                        myCollider.enabled = true;
                        onRestoreDrag = false;

                        onSwipingStack = false;

                        UpdateItemsInsideSatckAfterMove();

                        callback?.Invoke();
                    }));
                }
                else
                {
                    transform.position = newSlot.SlotPosition;
                    transform.SetParent(newSlot.transform);
                    myCollider.enabled = true;
                    onRestoreDrag = false;
                    onSwipingStack = false;

                    UpdateItemsInsideSatckAfterMove();

                    callback?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool HasEndedRestoreDrag()
        {
            return !onRestoreDrag ? true : false;
        }

        #endregion

        #region CORUTINES
        public IEnumerator AttachToPosition(Vector2 targetPosition, Action callbackAtEndPosition = null)
        {
            if (prepareToAttachOnSlot)
            {
                isAttachedToSlot = true;
            }

            positionLerper.SetValues(transform.position, targetPosition, true);

            while (!positionLerper.Reached)
            {
                positionLerper.Update();

                transform.position = positionLerper.CurrentValue;

                yield return null;
            }

            transform.position = positionLerper.CurrentValue;

            callbackAtEndPosition?.Invoke();

            yield return null;
        }
        #endregion

        public void OnPointerExit(PointerEventData eventData)
        {
            OnHoverSelection.Invoke("", false);
            Debug.Log("Exit item");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (SizeStack <= 0)
                return;
            OnHoverSelection.Invoke(stackedItems[0].ItemModelView.itemDescription, true);
            Debug.Log("Enter item");
        }
    }
}