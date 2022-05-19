using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.UI;

public class Furnace : MonoBehaviour
{
    [SerializeField] private float maxTimeToBurn;
    [SerializeField] private UIFurnace uiFurnace;
    [SerializeField] private EnergyHandler energyHandler = null;
    [SerializeField] private GameObject feedbackFurance = null;

    private float timerBurn;
    private bool isProcessing;

    private bool playerIsNear = false;

    private Action OnItemProcessed = null;

    private ItemModel itemPorcessed = null;
    private ItemView itemProcessing = null;

    void Start()
    {
        uiFurnace.IsFurnaceActive = IsProcessing;
        uiFurnace.OnProcessMaterial = SetProcess;
        uiFurnace.onCancelProcess += ResetProcessing;
        OnItemProcessed = uiFurnace.OnEndProcess;

        isProcessing = false;
        timerBurn = 0.0f;

        uiFurnace.SetDurationProcess(maxTimeToBurn);
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            uiFurnace.TogglePanel();
        }

        if (isProcessing)
        {
            if (timerBurn < maxTimeToBurn)
            {
                timerBurn += Time.deltaTime;

                energyHandler.ConsumeEnergyByProcess();

                uiFurnace.UpdateProgressFill(timerBurn);
            }
            else
            {
                timerBurn = 0;
                isProcessing = false;

                uiFurnace.GenerateProcessedItem(itemProcessing);
                itemProcessing = null;

                OnItemProcessed?.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!feedbackFurance.gameObject.activeSelf)
            {
                feedbackFurance.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = true;
        }    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (feedbackFurance.gameObject.activeSelf)
            {
                feedbackFurance.gameObject.SetActive(false);

                playerIsNear = false;
            }
        }
    }

    private void ResetProcessing()
    {
        timerBurn = 0f;
        isProcessing = false;
    }

    private bool IsProcessing()
    {
        return isProcessing;
    }

    private void SetProcess(ItemView item)
    {
        isProcessing = true;

        itemProcessing = item;
        itemPorcessed = uiFurnace.InverntoryController.GetItemModelFromId(itemProcessing.ItemType);

        energyHandler.SetCostOfProcessDecrement(itemPorcessed.costByProcess, 2f);

        Debug.Log("Processing " + isProcessing);
    }
}
