using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Interfaces;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

using Pathfinders.Toolbox.Lerpers;

public class ItemBase : MonoBehaviour, IDraggable
{
    #region EXPOSED_FIELDS
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private Image iconAttach = null;
    [SerializeField] private string nameItem = string.Empty;
    [SerializeField] private string description = string.Empty;
    [SerializeField] private int value = 0;

    [SerializeField] private Vector3Lerper.SMOOTH_TYPE typeLerp = default;
    [SerializeField] private float followSpeed = 0;
    #endregion

    #region PRIVATE_FIELDS
    private RectTransform draggingPlane;
    private bool isDragging = false;

    private PointerEventData eventData = null;

    private float currentLerpTime = 0;
    private float lerpTime = 0;

    private float finalPerc = 0;
    #endregion

    #region PROPERTIES
    public bool Dragged => isDragging;
    #endregion

    #region INITIALIZATION
    private void Start()
    {
        lerpTime = followSpeed;
    }

    private void Update()
    {
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

        gameObject.transform.SetParent(canvas.transform, false);
        gameObject.transform.SetAsLastSibling();

        draggingPlane = canvas.transform as RectTransform;

        Debug.Log("Empezo drag");

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

        Debug.Log("Clacula drag");
        this.eventData = eventData;

        RectTransform objectDragging = gameObject.transform as RectTransform;
        Vector3 globalMouse = default;

        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMouse))
        {
            objectDragging.position = Vector3.Lerp(objectDragging.position, globalMouse, finalPerc);
            objectDragging.rotation = draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Termino drag");

        this.eventData = eventData;
        currentLerpTime = 0;
        isDragging = false;
    }

    #endregion
}
