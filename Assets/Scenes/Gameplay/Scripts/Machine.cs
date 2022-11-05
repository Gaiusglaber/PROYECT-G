using System;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.UI;
using ProyectG.Common.Modules.Audio.Channels.Sound;

namespace ProyectG.Gameplay.Objects
{
	#region CLASSES
	public class Machine : MonoBehaviour
	{
        #region EXPOSED_FIELDS
        [SerializeField] protected SoundHandlerChannel soundsChannel = null;
        [SerializeField] protected List<string> animatorTriggers = null;
        [SerializeField] protected Animator animator = null;
		#endregion

		#region PRIVATE_FIELDS
		public Action OnInteract = null;

        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLS
        #endregion

        #region PUBLIC_METHODS
        public virtual void Init(BaseView viewAttach)
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public virtual void TriggerAnimation(string triggerId)
        {
            if (animator == null) return;

            string trigger = animatorTriggers.Find(trigger => trigger == triggerId);

            if(trigger == null)
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