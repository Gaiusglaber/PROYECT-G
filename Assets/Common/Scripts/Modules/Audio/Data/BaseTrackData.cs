using UnityEngine;

namespace ProyectG.Common.Modules.Audio.Data
{
    [CreateAssetMenu(fileName = "BaseTrackData", menuName = "ScriptableObjects/Base/BaseTrackData", order = 1)]
    [System.Serializable]
    public class BaseTrackData : ScriptableObject
    {
        public string id = string.Empty;
        public AudioClip clip = null;
        [Range(0, 1)] public float volume = 0;
        [Range(-3, 3)] public float pitch = 0;
        public bool loop = false;
    }
}