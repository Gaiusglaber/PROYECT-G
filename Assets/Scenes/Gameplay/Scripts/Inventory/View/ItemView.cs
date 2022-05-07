using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;
using ProyectG.Gameplay.Objects.Inventory.Data;

using Pathfinders.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ItemView : MonoBehaviour, IDraggable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float followSpeed = 0;
        [SerializeField] private Image itemImage = null;
        #endregion

        #region PRIVATE_FIELDS
        private Canvas canvas = null;

        private RectTransform draggingPlane;
        private bool isDragging = false;

        private PointerEventData eventData = null;

        private float currentLerpTime = 0;
        private float lerpTime = 0;
        private float finalPerc = 0;
        private Vector3Lerper positionLerper = null;

        private bool prepareToAttachOnSlot = false;
        private bool isAttachedToSlot = false;

        private Camera mainCamera = null;

        private (Vector2, Transform) slotPositionAttached = default;

        float timeToGoBackSlot = 0.5f;
        float time = 0;

        private BoxCollider2D myCollider = null;

        private string itemId;
        #endregion

        #region PROPERTIES
        public string ItemType { get { return itemId; } }
        public bool Dragged => isDragging;
        #endregion

        #region INITIALIZATION
        public void GenerateItem(Canvas mainCanvas, ItemModel itemData, SlotInventoryView slotAttached,Action onEndDrag = null)
        {
            time = 0;
            timeToGoBackSlot = 0.5f;

            canvas = mainCanvas;

            myCollider = GetComponent<BoxCollider2D>();

            mainCamera = Camera.main;

            lerpTime = followSpeed;

            itemId = itemData.itemId;
            itemImage.sprite = itemData.itemSprite;
            
            positionLerper = new Vector3Lerper(followSpeed * 0.5f, Vector3Lerper.SMOOTH_TYPE.STEP_SMOOTHER);

            //Aplica el lerp del nuevo item creado al slot al que fue enviado
            AttachToSlot(slotAttached.SlotPosition, slotAttached.gameObject.transform);
        }
        #endregion

        #region INTERACTION
        /// <summary>
        /// El update termina de desplazar el objeto si este no alcanzo el mouse hasta el final
        /// </summary>
        private void Update()
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

            if (!Dragged)
                return;

            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float perc = currentLerpTime / lerpTime;
            float smooth = perc;
            smooth = perc * perc;
            perc = smooth;

            finalPerc = perc;

            SetDraggedPosition(eventData);
        }
        #endregion

        #region DRAG_IMPLEMENTATION
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
                objectDragging.position = Vector3.Lerp(objectDragging.position, globalMouse, finalPerc);
                objectDragging.rotation = draggingPlane.rotation;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.eventData = eventData;
            currentLerpTime = 0;
            isDragging = false;

            prepareToAttachOnSlot = true;
        }

        public bool AttachToSlot(Vector2 positionSlot, Transform parent)
        {
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