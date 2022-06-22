using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.UI;

public class UpgradeTable : MonoBehaviour
{
    [SerializeField] private UIUpgradeTable uiUpgradeTable = null;
    [SerializeField] private EnergyHandler energyHandler = null;
    [SerializeField] GameObject feedbackUpgradeTable;
    [SerializeField] private Separator separator;

    private bool playerIsNear = false;

    void Start()
    {
        uiUpgradeTable.UnlockSeparator += EnableSeparator;
        playerIsNear = false;
    }

    private void OnDisable()
    {
        uiUpgradeTable.UnlockSeparator -= EnableSeparator;
    }

    void Update()
    {
        if(playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            uiUpgradeTable.TogglePanel();
        }
    }

    private void EnableSeparator(bool state)
    {
        energyHandler.ConsumeEnergyByUnlock();
        separator.SetIsEnabled(state);
        //Llamar evento para habilitar el separador
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!feedbackUpgradeTable.gameObject.activeSelf)
            {
                feedbackUpgradeTable.gameObject.SetActive(true);
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
            if (feedbackUpgradeTable.gameObject.activeSelf)
            {
                feedbackUpgradeTable.gameObject.SetActive(false);

                playerIsNear = false;
            }
        }
    }
}
