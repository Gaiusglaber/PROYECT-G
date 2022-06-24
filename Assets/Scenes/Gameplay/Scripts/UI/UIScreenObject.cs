using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class UIScreenObject : MonoBehaviour
{
    [SerializeField] private GameObject screenPanel;

    public void TogglePanel()
    {
        screenPanel.gameObject.SetActive(!screenPanel.activeSelf);
    }
}
