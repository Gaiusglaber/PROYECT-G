using System;
using UnityEngine;

namespace ProyectG.Common.Modules.Audio.Channels.Sound
{
    [CreateAssetMenu(fileName = "SoundHandlerChannel", menuName = "ScriptableObjects/Sound/Channels/SoundHandlerChannel", order = 1)]
    public class SoundHandlerChannel : ScriptableObject
    {
        #region ACTIONS
        public Action<string> OnPlaySound = null;
        public Action<string> OnStopSound = null;
        public Func<string, bool> IsSoundPlaying = null;
        #endregion
    }
}