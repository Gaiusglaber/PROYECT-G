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
    #region EXPOSED_FIELDS
    [SerializeField] private string id = string.Empty;
    [SerializeField] private List<ConversationData> ids = null;
    [SerializeField] private GameObject feedbackUpgradeTable = null;
    [SerializeField] private DialogPlayer dialogPlayer = null;

    [Header("DEBUG")]
    [SerializeField] private bool debugIndexActive = false;
    [SerializeField] private int debugIndex = 0;
    #endregion

    private int dialogIndex = 0;

    public Action OnIndexUpdated { get; set; }
    public string Id { get { return id; } }
    public List<ConversationData> Ids { get => ids; set => ids = value; }

    #region EXPOSED_FIELDS
    public void Init()
    {
        if (debugIndexActive)
        {
            dialogIndex = debugIndex;
        }
    }
    
    public void IncreaseDialogIndex(bool toggle)
    {
        if (toggle)
        {
            dialogIndex++;
            OnIndexUpdated?.Invoke();
        }
    }
    
    public void SetDialogIndex(int index)
    {
        dialogIndex = index;
    }
    #endregion

    #region PRIVATE_METHODS
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
            if (dialogIndex <= ids.Count - 1)
            {
                dialogPlayer.PlayDialog(ids[dialogIndex].conversationId);
                Debug.Log("Interaction");
            }
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
    #endregion
}
