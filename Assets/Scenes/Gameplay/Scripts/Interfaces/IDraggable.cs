using System.Collections;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Interfaces
{
    public interface IDraggable : IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Esta interfaz implementa de forma directa todo lo necesario para dragear algo.
        /// </summary>
        /// 
        public bool AttachToSlot(Vector2 positionSlot,Vector2Int gridPos,Transform parent, params ItemType[] allowedTypes);
    }

    public interface ISwitchable
    {
        public bool SwipeStackSlots(SlotInventoryView newSlot, Action callback = null);
    }
}