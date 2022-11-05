using System;
using UnityEngine;

namespace ProyectG.Common.Modules.Audio.Channels.Music
{
    [CreateAssetMenu(fileName = "MusicHandlerChannel", menuName = "ScriptableObjects/Music/Channels/MusicHandlerChannel", order = 2)]
    public class MusicHandlerChannel : ScriptableObject
    {
        #region ACTIONS
        public Action<string> OnPlayMusic = null;
        public Action<string> OnStopMusic = null;
        public Func<string, bool> IsMusicPlaying = null;
        #endregion
    }
}
