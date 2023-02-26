using UnityEngine;

using ProyectG.Common.UI.Dialogs;

public class NPC : MonoBehaviour
{
    [SerializeField] private string id = string.Empty;
    [SerializeField] private string[] ids = null;
    [SerializeField] private GameObject feedbackUpgradeTable = null;
    [SerializeField] private DialogPlayer dialogPlayer = null;

    private int dialogIndex = 0;

    public string Id { get { return id; } }
    public string[] Ids { get => ids; set => ids = value; }
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
