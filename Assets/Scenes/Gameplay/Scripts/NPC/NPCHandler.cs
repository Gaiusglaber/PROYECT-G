using System;
using UnityEngine;

public class NPCHandler : MonoBehaviour
{
    [SerializeField] private NPC[] npcs = null;

    public void Init(Action<string> onInteract)
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            npcs[i].Init(onInteract);
        }
    }

}
