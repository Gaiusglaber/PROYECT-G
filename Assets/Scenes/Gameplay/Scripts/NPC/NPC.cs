using ProyectG.Common.UI.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string[] ids = null;
    [SerializeField] private GameObject feedbackUpgradeTable = null;
    [SerializeField] private DialogPlayer dialogPlayer = null;

    private Action<string> onInteract = null;
    private int dialogIndex = 0;

    public string[] Id { get => ids; set => ids = value; }

    public void Init(Action<string> onInteract)
    {
        this.onInteract = onInteract;
    }
    public void IncreaseDialogIndex()
    {
        dialogIndex++;
    }
    public void SetDialogIndex(int index)
    {
        dialogIndex = index;
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
            dialogPlayer.PlayDialog(ids[dialogIndex]);
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
