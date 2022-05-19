using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.Objects.Inventory.Data
{
    [Serializable]
    public enum ItemType
    {
        fuel,
        common
    }
    [Serializable]
    [CreateAssetMenu(fileName = "ItemModel", menuName = "ScriptableObjects/Data/Items", order = 0)]
    public class ItemModel : ScriptableObject
    {
        #region PROTECTED_FIELDS
        [Header("ITEM MODEL SETTINGS")]
        public string itemId = string.Empty;
        public Sprite itemSprite = null;

        [TextArea(1,8)]
        public string itemDescription = string.Empty;
        public int itemValue = 0;
        public int energyCost = 0;
        public float timeToComplete = 0;
        public float costInterval = 0;

        public ItemType itemType;

        public List<ItemModel> itemResults = new List<ItemModel>();
        #endregion
    }
}