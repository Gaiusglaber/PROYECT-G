using System;

using UnityEngine;

using ProyectG.Common.Modules.Audio.Objects;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Common.Modules.Audio.Data.Sound;

public class Furnace : Machine
{
    #region EXPOSED_FIELDS
    [SerializeField] private UIFurnace uiFurnace;
    [SerializeField] private EnergyHandler energyHandler = null;
    [Header("Sound Data")]
    [Space(20)]
    [SerializeField] private AudioSourceObject customSource = null;
    [SerializeField] private SoundTrackData fireSoundData = null;
    #endregion

    #region PRIVATE_FIELDS
    private int energyGenerated = 0;
    private float timerFuel = 0;
    private float timeToBurnOutFuel = 0;

    private float timerProcess;
    private float timeToProcessObject = 0;
    private bool burningFuel = false;
    private bool isProcessing;

    private Action OnItemProcessed = null;
    private Action OnFuelEnergyBurned = null;

    private Action OnFuelItemConsumed = null;
    private Action OnItemConsumed = null;

    private ItemModel itemPorcessed = null;
    #endregion

    #region UNITY_CALLS
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

        customSource.Configure(fireSoundData, true);

        isInitialized = true;
    }

    protected override void Update()
    {
        base.Update();

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
                uiFurnace.GenerateProcessedItem(itemPorcessed, (state) => 
                {
                    if(state)
                    {
                        timerProcess = 0;
                        isProcessing = false;

                        itemPorcessed = null;

                        OnItemProcessed?.Invoke();

                        Debug.Log("Item processed successfully");
                    }
                    else
                    {
                        Debug.Log("The machine has in the output a item of different type, remove it and you will can process more items.");
                    }
                });
            }
        }

        if (!burningFuel)
            return;

        BurnFuel();
    }    
    #endregion

    #region PRIVATE_METHODS
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
        if(item != null)
        {
            if(item.itemResults.Count < 1)
            {
                Debug.LogWarning("The item model hasn't any result to process");
                return;
            }
        }
        else
        {
            Debug.LogWarning("The item model to process is null");
            return;
        }
        
        isProcessing = true;

        itemPorcessed = item;

        timeToProcessObject = itemPorcessed.timeToComplete;
        uiFurnace.SetDurationProcess(timeToProcessObject);

        energyHandler.SetCostOfProcessDecrement(itemPorcessed.energyCost, itemPorcessed.costInterval);

        OnItemConsumed?.Invoke();
    }
    #endregion
}
