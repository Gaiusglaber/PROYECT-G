using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Common.Modules.Audio.Data.Sound;

public class Opposite : Machine
{
    [SerializeField] private UIOppositeMachine uiOppositeMachine;
    [SerializeField] private EnergyHandler energyHandler = null;

    //private bool playerIsNear;

    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();

        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            uiOppositeMachine.TogglePanel();
        }
    }
}
