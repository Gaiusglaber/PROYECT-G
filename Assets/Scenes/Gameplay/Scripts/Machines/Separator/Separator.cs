using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.UI;

using TMPro;

public class Separator : Machine
{
    [SerializeField] private UISeparator uiSeparator;
    [SerializeField] private EnergyHandler energyHandler = null;
    [SerializeField] private TMP_Text feedbackSeparator = null;
    private float timerProcess;
    private float timeToProcessObject = 0;
    private float timeToProcessObject2 = 0;

    [SerializeField] private bool isProcessing;

    private bool playerIsNear;
    private bool isEnabled;

    private ItemModel itemProcessed = null;

    //Cambiar nombre de eventos y todos los atributos con respecto a "processed" a "separated" para entender mejor el codigo
    private Action OnItemProcessed = null;
    //private Action OnFuelBurned = null;

    void Start()
    {
        uiSeparator.IsSeparatorProcessing = IsProcessing;

        uiSeparator.OnProcessMaterial = SetProcess;

        uiSeparator.onCancelProcess += ResetProcessing;
        OnItemProcessed = uiSeparator.OnEndProcess;

        isProcessing = false;
        timerProcess = 0.0f;

        feedbackSeparator.gameObject.SetActive(false);

        isEnabled = false;
    }

    void Update()
    {
        if(playerIsNear && Input.GetKeyDown(KeyCode.E) && isEnabled)
        {
            uiSeparator.TogglePanel();
        }

        if (isProcessing)
        {
            if(timerProcess < timeToProcessObject)
            {
                timerProcess += Time.deltaTime;

                energyHandler.ConsumeEnergyByProcess();

                //uiSeparator.UpdateProgressFill(timerProcess);

                //llamar al metodo llamado UpdateProgressFill(timerProcess); en UISeparator

                Debug.Log("Item processing");
            }
            else
            {
                timerProcess = 0;
                isProcessing = false;

                uiSeparator.GenerateProcessedItems(itemProcessed);
                itemProcessed = null;

                //llamar al metodo GenerateProcessedItem(itemProcessed); y ver como generamos dos en vez de uno

                itemProcessed = null;

                //Invocar al evento OnTemProcessed
                OnItemProcessed?.Invoke();

                Debug.Log("Item processed successfully");
            }
        }
    }

    public void SetIsEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!feedbackSeparator.gameObject.activeSelf)
            {
                feedbackSeparator.gameObject.SetActive(true);
            }

            feedbackSeparator.text = isEnabled ? "<color=yellow><size=.3>E</size></color> to Interact" : "You need to <color=red><size=.3>unlock</size></color> this";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (feedbackSeparator.gameObject.activeSelf)
            {
                feedbackSeparator.gameObject.SetActive(false);

                playerIsNear = false;
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

    private void ResetProcessing()
    {
        timerProcess = 0f;
        isProcessing = false;
    }

    private bool IsProcessing()
    {
        return isProcessing;
    }

    private void SetProcess(ItemModel item)
    {
        isProcessing = true;

        itemProcessed = item;

        //setea el tiempo de duracion del item
        timeToProcessObject = itemProcessed.timeToComplete;
        uiSeparator.SetDurationProcess(timeToProcessObject);

        //consume energia
        energyHandler.SetCostOfProcessDecrement(itemProcessed.energyCost, itemProcessed.costInterval);
    }

    private void StartProcess()
    {
        isProcessing = true;
    }
}