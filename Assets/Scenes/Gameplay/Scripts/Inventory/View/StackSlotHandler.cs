using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;

using ProyectG.Toolbox.Lerpers;

using TMPro;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class StackSlotHandler : MonoBehaviour, IDraggable, ISwitchable
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

        private (Vector2,Vector2Int,Transform) slotPositionAttached = default;
        private (Vector2,Vector2Int,Transform) lastslotPositionAttached = default;

        private SlotInventoryView actualSlot = null;

        float timeToGoBackSlot = 0.5f;
        float time = 0;

        private BoxCollider2D myCollider = null;

        private Stack<ItemView> stackedItems = new Stack<ItemView>();
        #endregion

        #region PROPERTIES
        public bool Dragged => isDragging;
        public int SizeStack => stackedItems.Count;
        public string StackAmount { get { return stackAmount.text; } set { stackAmount.text = value; } }
        public SlotInventoryView ActualSlot { get { return actualSlot; } }
        public (Vector2, Vector2Int, Transform) SlotPositionAttached { get { return slotPositionAttached; } }
        public (Vector2, Vector2Int, Transform) LastslotPositionAttached { get { return lastslotPositionAttached; } }
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            RestorePosition();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (isDragging) { return; }

            if (collision.TryGetComponent(out SlotInventoryView slotView))
            {
                if(!isAttachedToSlot)
                {
                    AttachToSlot(slotView.SlotPosition, slotView.GridPosition ,slotView.transform);
                    isAttachedToSlot = true;
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Canvas mainCanvas, SlotInventoryView slotAttached)
        {
            positionLerper = new Vector3Lerper(speedFollow, Vector3Lerper.SMOOTH_TYPE.STEP_SMOOTHER);

            canvas = mainCanvas;
            mainCamera = Camera.main;

            myCollider = GetComponent<BoxCollider2D>();

            slotPositionAttached.Item1 = slotAttached.SlotPosition;
            slotPositionAttached.Item2 = slotAttached.GridPosition;
            slotPositionAttached.Item3 = slotAttached.transform;

            actualSlot = slotAttached;

            StackAmount = string.Empty;

            if (!isAttachedToSlot)
            {
                AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2, slotPositionAttached.Item3);
                isAttachedToSlot = true;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (stackedItems.Count < 1)
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

            SetDraggedPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.eventData = eventData;
            isDragging = false;

            prepareToAttachOnSlot = true;
            onRestoreDrag = true;
        }

        public void StackItemsInside(List<ItemView> listOfItems)
        {
            for (int i = 0; i < listOfItems.Count; i++)
            {
                stackedItems.Push(listOfItems[i]);
            }

            StackAmount = SizeStack.ToString();
        }

        public List<ItemView> GetStackFormStack()   //no supe como llamarlo mejor xd
        {
            List<ItemView> allReturnedItems = new List<ItemView>();

            allReturnedItems.AddRange(stackedItems);
            stackedItems.Clear();

            StackAmount = string.Empty;

            return allReturnedItems;
        }
        #endregion

        #region PRIVATE_METHODS
        private void RestorePosition()
        {
            if (onSwipingStack || !onRestoreDrag)
                return;

            if (isAttachedToSlot && prepareToAttachOnSlot)
            {
                if (time < timeToGoBackSlot)
                    time += Time.deltaTime;
                else
                {
                    time = 0;
                    prepareToAttachOnSlot = false;
                    myCollider.enabled = false;
                    AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2, slotPositionAttached.Item3);
                }
            }
        }

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

        public bool AttachToSlot(Vector2 positionSlot, Vector2Int gridPos,Transform parent)
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
                        transform.SetParent(parent);
                        myCollider.enabled = true;
                        onRestoreDrag = false;
                    }));
                }
                else
                {
                    transform.position = positionSlot;
                    transform.SetParent(parent);
                    myCollider.enabled = true;
                    onRestoreDrag = false;
                }

                return true;
            }

            return false;
        }

        public bool SwipeStackSlots(SlotInventoryView newSlot, Action<SlotInventoryView> callback = null)
        {
            myCollider.enabled = false;
            onSwipingStack = true;

            SlotInventoryView auxSlot = actualSlot;

            if(!isDragging)
            {
                actualSlot = newSlot;

                slotPositionAttached.Item1 = newSlot.SlotPosition;
                slotPositionAttached.Item2 = newSlot.GridPosition;
                slotPositionAttached.Item3 = newSlot.transform;

                if(useLerpAnimations)
                {
                    StartCoroutine(AttachToPosition(newSlot.SlotPosition, ()=> {

                        actualSlot.StackHandler = this;
                        transform.SetParent(newSlot.transform);
                        myCollider.enabled = true;
                        onRestoreDrag = false;

                        onSwipingStack = false;

                        callback?.Invoke(auxSlot);
                    }));
                }
                else
                {
                    actualSlot.StackHandler = this;
                    transform.position = newSlot.SlotPosition;
                    transform.SetParent(newSlot.transform);
                    myCollider.enabled = true;
                    onRestoreDrag = false;
                    onSwipingStack = false;
                    callback?.Invoke(auxSlot);
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
    }
}