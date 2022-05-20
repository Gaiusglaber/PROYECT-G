using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;

namespace ProyectG.Player.Animation
{
    public class PlayerAnim : MonoBehaviour
    {
        #region PRIVATE_FIELDS
        private PlayerController playerController = null;
        private Animator playerAnimator = null;
        private SpriteRenderer playerSprite = null;
        #endregion

        #region UNITY_CALL
        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            playerAnimator = GetComponent<Animator>();
            playerSprite = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (playerController == null || playerAnimator == null || playerSprite == null)
                return;

            if(playerController.Velocity.x != 0)
            {
                playerAnimator.SetBool("Walking", true);
            }
            else
            {
                playerAnimator.SetBool("Walking", false);
            }

            if(playerController.Velocity.y > 0)
            {
                playerAnimator.SetFloat("Jump",0.5f);
            }
            else if(playerController.Velocity.y < 0)
            {
                playerAnimator.SetFloat("Jump", 1f);
            }
            else if(playerController.Velocity.y == 0)
            {
                playerAnimator.SetFloat("Jump", 0f);
            }

            if (playerController.Velocity.x < 0)
                playerSprite.flipX = true;
            else
                playerSprite.flipX = false;
        }
        #endregion
    }
}