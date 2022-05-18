using System;
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
        public string itemId = string.Empty;
        public Sprite itemSprite = null;

        [TextArea(1,8)]
        public string itemDescription = string.Empty;
        public int itemValue = 0;

        public ItemType itemType;

        #endregion
    }
}