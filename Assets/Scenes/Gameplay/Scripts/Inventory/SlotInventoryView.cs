using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class SlotInventoryView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Image slotFrame = null;
        [SerializeField] private Image iconItemAttach = null;
        #endregion

        #region PRIVATE_FIELDS
        #endregion

        #region PROPERTIE
        public Vector2 SlotPosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }
        #endregion
    }
}