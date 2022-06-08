using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Interfaces;

using ProyectG.Player.Controller;

using DragonBones;

namespace ProyectG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private PlayerController playerController = null;
        [Header("---ANIMATIONS---")]
        [SerializeField] private UnityArmatureComponent customAnimator = null;

        private string lastAnimationExecuted = string.Empty;

        private bool playerHasDoAttack = false;
        private float timeToResetAttack = 0.75f;
        private float timer = 0f;

        public bool PlayerHasDoAttack { get { return playerHasDoAttack; } }

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
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerHasDoAttack = true;

                SetAnimation("ataque");

                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
                foreach (Collider2D hit in hits)
                {
                    if (hit.TryGetComponent(out IHittable hittable))
                    {
                        hittable.OnHit();
                    }
                }
            }
        }
        private void SetAnimation(string idAnimation, int playTimes = 1)
        { 
            customAnimator.animation.Play(idAnimation, playTimes);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
