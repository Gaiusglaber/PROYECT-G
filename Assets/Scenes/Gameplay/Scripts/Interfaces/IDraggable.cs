using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProyectG.Gameplay.Interfaces
{
    public interface IDraggable : IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Esta interfaz implementa de forma directa todo lo necesario para dragear algo.
        /// </summary>
        /// 
        public void AttachToSlot(Vector2 positionSlot, Transform parent);
    }
}