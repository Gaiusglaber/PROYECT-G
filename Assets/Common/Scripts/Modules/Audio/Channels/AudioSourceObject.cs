using UnityEngine;

using ProyectG.Common.Modules.Audio.Data;

using ProyectG.Toolbox.Pooling;
using System;
using UnityEditor;

namespace ProyectG.Common.Modules.Audio.Objects
{
    public class AudioSourceObject : Pooleable
    {
        #region PRIVATE_FIELDS
        private BaseTrackData sourceData = null;
        private AudioSource audioSource = null;
        private Action onTrackEnded = null;
        #endregion

        #region PROPERTIES
        public BaseTrackData SourceData { get { return sourceData; } }
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (audioSource == null || audioSource.clip == null) return;

            if(!audioSource.isPlaying)
            {
                onTrackEnded?.Invoke();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Configure(BaseTrackData trackData, bool playInstant = false, Action onTrackEnded = null)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null) return;

            audioSource.volume = trackData.volume;
            audioSource.clip = trackData.clip;
            audioSource.pitch = trackData.pitch;
            audioSource.loop = trackData.loop;

            this.onTrackEnded = onTrackEnded;

            sourceData = trackData;

            if (playInstant)
            {
                if(!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }

        public void Play()
        {
            if (audioSource == null || audioSource.clip == null) return;

            audioSource.Play();
        }

        public void Stop()
        {
            if (audioSource == null || audioSource.clip == null) return;

            audioSource.Stop();
        }

        public void Pause()
        {
            if (audioSource == null || audioSource.clip == null) return;

            audioSource.Pause();
        }

        public void Resume()
        {
            if (audioSource == null || audioSource.clip == null) return;

            audioSource.UnPause();
        }

        public void UpdateVolume(float volume)
        {
            if (audioSource == null) return;

            audioSource.volume = volume;
        }

        public void UpdatePitch(float pitch)
        {
            if (audioSource == null) return;

            audioSource.pitch = pitch;
        }
        #endregion

        #region OVERRIDE
        public override void Get()
        {
            gameObject.SetActive(true);
        }

        public override void Release()
        {
            gameObject.SetActive(false);

            ClearSource();
        }
        #endregion

        #region PRIVATE_METHODS
        private void ClearSource()
        {
            if (audioSource == null) return;

            audioSource.Stop();

            audioSource.clip = null;
            audioSource.volume = 0f;
            audioSource.pitch = 1f;
            audioSource.loop = false;

            onTrackEnded = null;
            sourceData = null;
        }
        #endregion
    }
}