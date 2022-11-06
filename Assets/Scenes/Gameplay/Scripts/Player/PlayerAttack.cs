using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Interfaces;

using ProyectG.Player.Controller;

using DragonBones;
using ProyectG.Common.Modules.Audio.Channels.Sound;

namespace ProyectG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private PlayerController playerController = null;
        [SerializeField] private LayerMask layerToCheck;
        [Header("---ANIMATIONS---")]
        [SerializeField] private UnityArmatureComponent customAnimator = null;

        private string lastAnimationExecuted = string.Empty;

        private bool playerHasDoAttack = false;
        private float timeToResetAttack = 1f;
        private float timer = 0f;
        private SoundHandlerChannel soundChannel = null;

        public bool PlayerHasDoAttack { get { return playerHasDoAttack; } }

        #region UNITY_CALLS
        void Update()
        {
            if (!playerController.IsControllerEnable)
                return;

            if(playerHasDoAttack)
            {
                if(timer < timeToResetAttack)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0;
                    playerHasDoAttack = false;
                }

                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerHasDoAttack = true;

                SetAnimation("ataque");

                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

                Debug.Log("Hits: " + hits.Length);

                soundChannel.OnPlaySound?.Invoke("Attack");

                foreach (Collider2D hit in hits)
                {
                    if (hit.TryGetComponent(out IHittable hittable))
                    {
                        Debug.Log("Entro hit");
                        hittable.OnHit();
                    }
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(SoundHandlerChannel soundHandlerChannel)
        {
            soundChannel = soundHandlerChannel;
        }
        #endregion

        #region PRIVATE_METHODS
        private void SetAnimation(string idAnimation, int playTimes = 1)
        { 
            customAnimator.animation.Play(idAnimation, playTimes);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
        #endregion
    }
}
