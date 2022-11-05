using UnityEngine;

namespace ProyectG.Common.Modules.Audio.Data.Sound
{
    [CreateAssetMenu(fileName = "SoundTrackData", menuName = "ScriptableObjects/Sound/SoundTrackData", order = 1)]
    public class SoundTrackData : BaseTrackData
    {
        public bool releaseAfterPlayfack = false;
    }
}
