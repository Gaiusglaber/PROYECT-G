using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;
using UnityEngine.U2D.Animation;
using ProyectG.Gameplay.UI;

public class EnergeticSunflowers : MonoBehaviour
{
    [SerializeField] private EnergyHandler energyHandler;
    [SerializeField] private float timeToEarnEnergy;
    [SerializeField] private int energyEarnValue;

    private float timerEarnEnergy;
    void Start()
    {
        timerEarnEnergy = 0.0f;
        //energyHandler.SetValueOfFuelIncrement(energyEarnValue, timeToEarnEnergy);
    }

    void Update()
    {
        if (timerEarnEnergy <= timeToEarnEnergy)
            timerEarnEnergy += Time.deltaTime;
        else
        {
            energyHandler.UpdateEnergy(energyHandler.cantEnergy + energyEarnValue);
            timerEarnEnergy = 0.0f;
        }
    }
}
