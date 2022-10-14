using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects;

public class Furnace : Machine
{
    [SerializeField] private UIFurnace uiFurnace;
    [SerializeField] private EnergyHandler energyHandler = null;
    [SerializeField] private GameObject feedbackFurance = null;

    private int energyGenerated = 0;
    private float timerFuel = 0;
    private float timeToBurnOutFuel = 0;

    private float timerProcess;
    private float timeToProcessObject = 0;
    private bool burningFuel = false;
    private bool isProcessing;

    private bool playerIsNear = false;

    private Action OnItemProcessed = null;
    private Action OnFuelEnergyBurned = null;

    private Action OnFuelItemConsumed = null;
    private Action OnItemConsumed = null;

    private ItemModel itemPorcessed = null;

    void Start()
    {
        uiFurnace.IsFurnaceProcessing = IsProcessing;
        uiFurnace.IsFurnaceBurning = IsBurningFuel;

        uiFurnace.OnProcessMaterial = SetProcess;
        uiFurnace.OnProcessFuel = SetBurnFuel;

        uiFurnace.onCancelProcess += ResetProcessing;
        OnItemProcessed = uiFurnace.OnEndProcess;

        OnFuelEnergyBurned = uiFurnace.OnEndBurnOfFuel;
        OnFuelItemConsumed = uiFurnace.OnConsumeFuel;
        OnItemConsumed = uiFurnace.OnConsumeItem;

        isProcessing = false;
        timerProcess = 0.0f;        
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            uiFurnace.TogglePanel();
        }

        if (isProcessing)
        {
            if (timerProcess < timeToProcessObject)
            {
                timerProcess += Time.deltaTime;

                energyHandler.ConsumeEnergyByProcess();

                uiFurnace.UpdateProgressFill(timerProcess);

                Debug.Log("Item processing");
            }
            else
            {
                timerProcess = 0;
                isProcessing = false;

                uiFurnace.GenerateProcessedItem(itemPorcessed);
                itemPorcessed = null;

                OnItemProcessed?.Invoke();

                Debug.Log("Item processed successfully");
            }
        }

        if (!burningFuel)
            return;

        BurnFuel();
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

    private void BurnFuel()
    {
        if(timerFuel > 0)
        {
            timerFuel -= Time.deltaTime;

            energyHandler.IncreaseEnergyByFuel();

            uiFurnace.UpdateFillFuel(timerFuel, timeToBurnOutFuel);
        }
        else
        {
            burningFuel = false;

            OnFuelEnergyBurned?.Invoke();
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

    private bool IsBurningFuel()
    {
        return burningFuel;
    }

    private void SetBurnFuel(FuelItem fuelModel)
    {
        if (fuelModel == null)
            return;

        burningFuel = true;

        timeToBurnOutFuel = fuelModel.timeToBurnOut;
        energyGenerated = fuelModel.energyGenerated;
        timerFuel = timeToBurnOutFuel;

        energyHandler.SetValueOfFuelIncrement(energyGenerated, fuelModel.energyPerTime);

        OnFuelItemConsumed?.Invoke();
    }

    private void SetProcess(ItemModel item)
    {
        isProcessing = true;

        itemPorcessed = item;

        timeToProcessObject = itemPorcessed.timeToComplete;
        uiFurnace.SetDurationProcess(timeToProcessObject);

        energyHandler.SetCostOfProcessDecrement(itemPorcessed.energyCost, itemPorcessed.costInterval);

        OnItemConsumed?.Invoke();
    }
}
