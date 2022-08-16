using System;
using UnityEngine;
using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects
{
    public class WorldItem : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private SpriteRenderer spriteAttach = null;
        [SerializeField] protected WorldItemSO data = null;
        [SerializeField] protected float fallSpeed = 0;
        #endregion

        #region PRIVATE_FIELDS
        private Vector2Lerper posLerper = null;
        private Vector2Lerper sizeLerper = null;

        private Action<string> onTakedItem = null;

        private bool itemAddedToInventory = false;
        private bool worldItemTaked = false;

        private bool initialized = false;
        #endregion

        #region PROPERTIES
        public Action<string> OnTakedItem { get { return onTakedItem; } set { onTakedItem = value; } }
        #endregion

        #region UNITY_CALLS
        protected void Start()
        {
            posLerper = new Vector2Lerper(fallSpeed, AbstractLerper<Vector2>.SMOOTH_TYPE.STEP_SMOOTHER);
            sizeLerper = new Vector2Lerper(fallSpeed, Vector2Lerper.SMOOTH_TYPE.EXPONENTIAL);

            worldItemTaked = false;
            itemAddedToInventory = false;

            SetItemData();

            sizeLerper.SetValues(Vector2.zero, Vector2.one, true);

            initialized = true;
        }
        protected void Update()
        {
            if (!initialized)
                return;

            UpdateLerpers();
        }

        protected void OnDestroy()
        {
            initialized = false;
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !worldItemTaked)
            {
                posLerper.SetValues(transform.position, collision.transform.position, true);
                sizeLerper.SetValues(Vector2.one, Vector2.zero, true);
                worldItemTaked = true;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetOnItemTaked(Action<string> onItemTaked)
        {
            onTakedItem = onItemTaked;
        }
        #endregion

        #region PRIVATE_METHODS
        private void UpdateLerpers()
        {
            if (!worldItemTaked)
                return;

            if (posLerper.On)
            {
                posLerper.Update();
                transform.position = posLerper.CurrentValue;
            }
            if (sizeLerper.On)
            {
                sizeLerper.Update();
                transform.localScale = sizeLerper.CurrentValue;

                if (sizeLerper.Reached)
                {
                    if (!itemAddedToInventory)
                    {
                        onTakedItem?.Invoke(data.itemModel.itemId);
                        itemAddedToInventory = true;
                        Destroy(gameObject, 0.5f);
                    }
                }
            }
        }

        private void SetItemData()
        {
            spriteAttach.sprite = data.sprite;
        }
        #endregion
    }
}