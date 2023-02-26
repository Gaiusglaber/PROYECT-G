using System;
using UnityEngine;

public class NPCHandler : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private NPC[] npcs = null;
    #endregion

    #region PUBLIC_METHODS
    public void OnConversationEnd(string id,bool toggle)
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i].Id.Contains(id))
            {
                npcs[i].IncreaseDialogIndex(toggle);
            }
        }
    }
    #endregion

}
