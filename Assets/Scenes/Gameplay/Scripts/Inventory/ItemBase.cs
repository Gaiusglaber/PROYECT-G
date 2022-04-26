using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Interfaces;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Pathfinders.Toolbox.Lerpers;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemBase : MonoBehaviour, IDraggable
{
    #region EXPOSED_FIELDS
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private Image iconAttach = null;
    [SerializeField] private string nameItem = string.Empty;
    [SerializeField] private string description = string.Empty;
    [SerializeField] private int value = 0;

    [SerializeField] private float followSpeed = 0;
    #endregion

    #region PRIVATE_FIELDS
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
    #endregion

    #region PROPERTIES
    public bool Dragged => isDragging;
    #endregion

    #region INITIALIZATION
    public void Init(Action onEndDrag = null)
    {
        myCollider = GetComponent<BoxCollider2D>();

        mainCamera = Camera.main;

        lerpTime = followSpeed;

        positionLerper = new Vector3Lerper(followSpeed * 0.5f, Vector3Lerper.SMOOTH_TYPE.STEP_SMOOTHER);
    }
    private void Start()
    {
        Init();
    }
    #endregion

    #region INTERACTION
    /// <summary>
    /// El update termina de desplazar el objeto si este no alcanzo el mouse hasta el final
    /// </summary>
    private void Update()
    {
        if(isAttachedToSlot && prepareToAttachOnSlot)
        {
            if(time < timeToGoBackSlot)
                time += Time.deltaTime;
            else
            {
                time = 0;
                prepareToAttachOnSlot = false;
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

    public void AttachToSlot(Vector2 positionSlot, Transform parent)
    {
        if(!isDragging)
        {
            if (prepareToAttachOnSlot)
            {
                
                isAttachedToSlot = true;
                slotPositionAttached.Item1 = positionSlot;
                slotPositionAttached.Item2 = parent;
                myCollider.enabled = false;
            }

            StartCoroutine(AttachToPosition(positionSlot, ()=> { transform.SetParent(parent); myCollider.enabled = true; } ));
        }
    }
    #endregion

    #region CORUTINES
    private IEnumerator AttachToPosition(Vector2 targetPosition, Action callbackAtEndPosition = null)
    {
        if(prepareToAttachOnSlot)
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
