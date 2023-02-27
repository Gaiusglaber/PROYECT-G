using System;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Common.UI.Dialogs;

[Serializable]
public class ConversationData
{
    [SerializeField] public string conversationId;
    [SerializeField] public bool isItemCheck = false;
}

public class NPC : MonoBehaviour
{
    [SerializeField] private string id = string.Empty;
    [SerializeField] private List<ConversationData> ids = null;
    [SerializeField] private GameObject feedbackUpgradeTable = null;
    [SerializeField] private DialogPlayer dialogPlayer = null;

    private int dialogIndex = 0;

    public string Id { get { return id; } }
    public List<ConversationData> Ids { get => ids; set => ids = value; }
    public void IncreaseDialogIndex(bool toggle)
    {
        if (toggle)
        {
            dialogIndex++;
        }
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
            dialogPlayer.PlayDialog(ids[dialogIndex].conversationId);
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
