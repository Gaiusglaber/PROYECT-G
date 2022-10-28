using ProyectG.Common.UI.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string id = string.Empty;
    [SerializeField] private GameObject feedbackUpgradeTable = null;
    [SerializeField] private DialogPlayer dialogPlayer = null;

    private Action<string> onInteract = null;
    public string Id { get => id; set => id = value; }

    public void Init(Action<string> onInteract)
    {
        this.onInteract = onInteract;
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
        if (collision.CompareTag("Player")&& Input.GetKeyDown(KeyCode.E))
        {
            dialogPlayer.PlayDialog(id);
            Debug.Log("Interaction");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (feedbackUpgradeTable.gameObject.activeSelf)
            {
                feedbackUpgradeTable.gameObject.SetActive(false);
            }
        }
    }
}
