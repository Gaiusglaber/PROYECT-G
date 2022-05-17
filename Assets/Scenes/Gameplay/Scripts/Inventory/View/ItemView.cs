using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;
using ProyectG.Gameplay.Objects.Inventory.Data;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ItemView : MonoBehaviour, IDraggable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float followSpeed = 0;
        [SerializeField] private Image itemImage = null;
        [SerializeField] private bool useLerpAnimations = true;
        #endregion

        #region PRIVATE_FIELDS
        private Canvas canvas = null;
        private Camera mainCamera = null;
        private RectTransform draggingPlane;
        private BoxCollider2D myCollider = null;
        private Vector3Lerper positionLerper = null;
        private PointerEventData eventData = null;

        private float currentLerpTime = 0;
        private float lerpTime = 0;
        private float finalPerc = 0;
        private float timeToGoBackSlot = 0.5f;
        private float time = 0;

        private bool isDragging = false;
        private bool prepareToAttachOnSlot = false;
        private bool isAttachedToSlot = false;
        private bool dragDisable = false;
        private bool itemInitialized = false;

        private Action<Vector2Int, Vector2Int> onEndedChangeOfSlot = null;

        private (Vector2, Vector2Int ,Transform) slotPositionAttached = default;
        private Vector2Int slotGridPosition = default;

        private string itemId;

        private ItemType itemViewType;

        #endregion

        #region PROPERTIES
        public string ItemType { get { return itemId; } }
        public bool Dragged => isDragging;

        public (Vector2, Vector2Int, Transform) SlotPositionAttached { get { return slotPositionAttached; } }
        #endregion

        #region INITIALIZATION
        public void GenerateItem(Canvas mainCanvas, SlotInventoryView slotAttached, ItemModel itemData = null, Action<Vector2Int,Vector2Int> onEndDrag = null)
        {
            onEndedChangeOfSlot = onEndDrag;

            time = 0;
            timeToGoBackSlot = 0.5f;
            lerpTime = followSpeed;


            canvas = mainCanvas;
            myCollider = GetComponent<BoxCollider2D>();
            mainCamera = Camera.main;

            if(itemData != null)
            {
                itemId = itemData.itemId;
                itemImage.sprite = itemData.itemSprite;
                itemViewType = itemData.itemType;
            }
            else
            {
                itemId = "stacker";
            }

            //slotGridPosition = slotAttached.SlotPosition;

            positionLerper = new Vector3Lerper(followSpeed * 0.5f, Vector3Lerper.SMOOTH_TYPE.STEP_SMOOTHER);

            slotPositionAttached.Item1 = slotAttached.SlotPosition;
            slotPositionAttached.Item2 = slotAttached.GridPosition;
            slotPositionAttached.Item3 = slotAttached.transform;

            //Aplica el lerp del nuevo item creado al slot al que fue enviado
            itemInitialized = false;
            isAttachedToSlot = true;
            AttachToSlot(slotAttached.SlotPosition, slotAttached.GridPosition, slotAttached.gameObject.transform);
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
                    AttachToSlot(slotPositionAttached.Item1, slotPositionAttached.Item2, slotPositionAttached.Item3);
                }
            }

            if (!Dragged || dragDisable)
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
            if (dragDisable)
                return;

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

        private bool CheckItemType(ItemType[] allowedTypes)
        {
            bool check = false;
            Debug.Log("allowedTypes lenght: " + allowedTypes.Length);
            if (allowedTypes.Length < 1)
            {
                return true;
            }
            for(int i = 0; i < allowedTypes.Length; i++)
            {
                if (itemViewType == allowedTypes[i])
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public bool AttachToSlot(Vector2 positionSlot, Vector2Int gridPos, Transform parent, params ItemType[] allowedTypes)
        {
            Debug.Log("entro!");
            if (!CheckItemType(allowedTypes))
                return false;

            if (!isDragging)
            {
                (Vector2, Vector2Int, Transform) lastSlotPosition = slotPositionAttached;

                if (prepareToAttachOnSlot)
                {
                    isAttachedToSlot = true;
                    slotPositionAttached.Item1 = positionSlot;
                    slotPositionAttached.Item2 = gridPos;
                    slotPositionAttached.Item3 = parent;
                }

                if(useLerpAnimations)
                {
                    StartCoroutine(AttachToPosition(positionSlot, () =>
                    {
                        if(itemInitialized)
                        {
                            onEndedChangeOfSlot?.Invoke(lastSlotPosition.Item2, slotPositionAttached.Item2);
                        }

                        transform.SetParent(parent);
                        myCollider.enabled = true;
                        itemInitialized = true;
                    }));
                }
                else
                {
                    transform.position = positionSlot;

                    if (itemInitialized)
                    {
                        onEndedChangeOfSlot?.Invoke(lastSlotPosition.Item2, slotPositionAttached.Item2);
                    }

                    transform.SetParent(parent);
                    myCollider.enabled = true;
                    itemInitialized = true;
                }

                return true;
            }

            return false;
        }

        public void SwitchStateItem(bool state)
        {
            if(itemId == "stacker")
            {
                dragDisable = false;
                return;
            }

            dragDisable = state;
        }

        public void SwitchStateCollider(bool state)
        {
            myCollider.enabled = state;
            enabled = state;
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

        public bool SwipeStackSlots(Vector2 positionSlot, Vector2Int gridPos, Transform parent, Action callback = null)
        {
            //Nope;
            return false;
        }
        #endregion
    }
}