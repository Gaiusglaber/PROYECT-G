using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;

public class Furance : MonoBehaviour
{
    [SerializeField] private float maxTimeToBurn;
    [SerializeField] private UIFurance uiFurance;

    private List<ItemModel> furanceInventory = new List<ItemModel>();

    private float timerBurn;
    private bool isProcessing;

    private Action OnItemProcessed = null;

    void Start()
    {
        uiFurance.IsFurnanceActive = IsProcessing;
        uiFurance.OnProcessMaterial = SetProcess;
        isProcessing = false;
        timerBurn = 0.0f;
    }

    void Update()
    {
        if (isProcessing)
        {
            if (timerBurn <= maxTimeToBurn)
            {
                timerBurn += Time.deltaTime;
                Debug.Log("timer: " + timerBurn.ToString("F0"));
                //llenar la barra indicadora de quema de combustible
            }
            else
            {
                OnItemProcessed?.Invoke();
                timerBurn = 0.0f;
                isProcessing = false;
                
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
            uiFurance.ShowPanel(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiFurance.ShowPanel(true);
        }
    }

    private bool IsProcessing()
    {
        return isProcessing;
    }

    private void SetProcess(ItemView item, Action onProcessEnd)
    {
        isProcessing = true;
        OnItemProcessed = onProcessEnd;
    }
}
