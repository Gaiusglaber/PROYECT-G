using System;
using UnityEngine;

public class NPCHandler : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private NPC[] npcs = null;
    #endregion

    #region PUBLIC_METHODS
    public void Init()
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i] != null)
            {
                npcs[i].Init();
            }
        }
    }
    
    public void OnConversationEnd(string id,bool toggle)
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            ConversationData converData = npcs[i].Ids.Find(converData => converData.conversationId == id);
            if (converData != null)
            {
                if (!converData.isItemCheck && !toggle)
                {
                    npcs[i].IncreaseDialogIndex(true);
                }
                else
                {
                    npcs[i].IncreaseDialogIndex(toggle);
                }
            }
        }
    }
    #endregion
}
