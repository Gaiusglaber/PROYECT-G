using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;
using TMPro;

public class HoverSelection : MonoBehaviour
{
    [SerializeField] private RectTransform followObject;
    [SerializeField] private TMP_Text followText;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Camera cam;

    [SerializeField] private int distanceRay;


    void Start()
    {
        ToggleHoverSelection("", false);
    }

    void Update()
    {
        MoveObject();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * distanceRay, Color.yellow);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distanceRay))
        {
            Debug.Log("Entro en raycast hit!");
            Transform selection = hit.transform;
            if(selection.GetComponent<FuelItem>().itemId == "Wood")
            {
                //MoveObject();
                Debug.Log("Es Madera!");
            }
            else
            {
                Debug.Log("No es ningun item!");
            }
        }
    }

    public void ToggleHoverSelection(string description, bool state)
    {
        followObject.gameObject.SetActive(state);
        followText.text = description;
    }

    public void MoveObject()
    {
        Vector2 pos = default;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        parentCanvas.transform as RectTransform, Input.mousePosition,
        parentCanvas.worldCamera,
        out pos);
        followObject.position = parentCanvas.transform.TransformPoint(pos + offset);
    }
}
