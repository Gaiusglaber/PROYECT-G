using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;

using Pathfinders.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class StackSlotHandler : MonoBehaviour, IDraggable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float speedFollow = 0;

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

        private (Vector2, Transform) slotPositionAttached = default;

        float timeToGoBackSlot = 0.5f;
        float time = 0;

        private BoxCollider2D myCollider = null;

        private Stack<ItemView> stackedItems = new Stack<ItemView>();
        #endregion

        #region PROPERTIES
        public bool Dragged => isDragging;
        public int SizeStack => stackedItems.Count;
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            RestorePosition();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (isDragging)
            {
                return;
            }

            if(collision.TryGetComponent(out SlotInventoryView slotView))
            {
                if(!isAttachedToSlot)
                {
                    AttachToSlot(slotView.SlotPosition, slotView.transform);
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
            slotPositionAttached.Item2 = slotAttached.transform;

            if (!isAttachedToSlot)
            {
                AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2);
                isAttachedToSlot = true;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
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
            if (!Dragged || gameObject == null)
                return;

            SetDraggedPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.eventData = eventData;
            isDragging = false;

            prepareToAttachOnSlot = true;
        }
        
        public void StackItemsInside(List<ItemView> listOfItems)
        {
            for (int i = 0; i < listOfItems.Count; i++)
            {
                stackedItems.Push(listOfItems[i]);
            }
        }

        public List<ItemView> GetStackFormStack()   //no supe como llamarlo mejor xd
        {
            List<ItemView> allReturnedItems = new List<ItemView>();

            allReturnedItems.AddRange(stackedItems);
            stackedItems.Clear();

            return allReturnedItems;
        }
        #endregion

        #region PRIVATE_METHODS
        private void RestorePosition()
        {
            if (isAttachedToSlot && prepareToAttachOnSlot)
            {
                if (time < timeToGoBackSlot)
                    time += Time.deltaTime;
                else
                {
                    time = 0;
                    prepareToAttachOnSlot = false;
                    myCollider.enabled = false;
                    AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2);
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

        public bool AttachToSlot(Vector2 positionSlot, Transform parent)
        {
            if (parent == null)
            {
                AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2);
                return false;
            }

            if (!isDragging)
            {
                if (prepareToAttachOnSlot)
                {
                    isAttachedToSlot = true;
                    slotPositionAttached.Item1 = positionSlot;
                    slotPositionAttached.Item2 = parent;
                }

                StartCoroutine(AttachToPosition(positionSlot, () =>
                {
                    transform.SetParent(parent);
                    myCollider.enabled = true;
                }));

                return true;
            }

            return false;
        }
        #endregion

        #region CORUTINES
        private IEnumerator AttachToPosition(Vector2 targetPosition, Action callbackAtEndPosition = null)
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