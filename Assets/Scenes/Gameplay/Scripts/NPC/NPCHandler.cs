using System;
using UnityEngine;

public class NPCHandler : MonoBehaviour
{
    [SerializeField] private NPC[] npcs = null;

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

}
