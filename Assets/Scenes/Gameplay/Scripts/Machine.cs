using System;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.UI;
using ProyectG.Common.Modules.Audio.Channels.Sound;

using TMPro;

namespace ProyectG.Gameplay.Objects
{
    #region CLASSES
    public class Machine : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] protected SoundHandlerChannel soundsChannel = null;
        [SerializeField] protected List<string> animatorTriggers = null;
        [SerializeField] protected BaseView uiMachine = null;
        [SerializeField] protected Animator animator = null;
        [SerializeField] protected TMP_Text feedbackMachine = null;
        #endregion

        #region PRIVATE_FIELDS
        public Action OnInteract = null;
        protected bool playerIsNear;
        protected bool isInitialized = false;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLS
        protected virtual void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            if (playerIsNear && Input.GetKeyDown(KeyCode.E))
            {
                uiMachine.TogglePanel();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (feedbackMachine != null)
                {
                    if (!feedbackMachine.gameObject.activeSelf)
                    {
                        
                        feedbackMachine.gameObject.SetActive(true);
                    }
                    feedbackMachine.text = "<color=yellow><size=.3>E</size></color> to Interact";
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (feedbackMachine != null)
                {
                    if (feedbackMachine.gameObject.activeSelf)
                        
                    {
                        feedbackMachine.gameObject.SetActive(false);
                        playerIsNear = false;
                    }
                }
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                playerIsNear = true;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public virtual void Init(BaseView viewAttach)
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            uiMachine = viewAttach;
            uiMachine?.Init();
            isInitialized = true;
        }

        public virtual void TriggerAnimation(string triggerId)
        {
            if (animator == null) return;

            string trigger = animatorTriggers.Find(trigger => trigger == triggerId);

            if (trigger == null)
            {
                Debug.Log("That triggerID is not on the trigger list of this machine");
                return;
            }

            animator.SetTrigger(trigger);
        }

        public virtual void TriggerSoundEffect(string idSound)
        {
            if (soundsChannel == null) return;

            soundsChannel.OnPlaySound?.Invoke(idSound);
        }
        #endregion

        #region PRIVATE_METHODS
        #endregion

        #region PUBLIC_CORROUTINES
        #endregion

        #region PRIVATE_CORROUTINES
        #endregion

    }
    #endregion

}