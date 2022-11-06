using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Common.Modules.Audio.Objects;
using ProyectG.Common.Modules.Audio.Data.Sound;
using ProyectG.Common.Modules.Audio.Data.Music;
using ProyectG.Common.Modules.Audio.Channels.Music;
using ProyectG.Common.Modules.Audio.Channels.Sound;

using ProyectG.Toolbox.Singletons;
using ProyectG.Toolbox.Pooling;

namespace ProyectG.Common.Modules.Audio.Handlers
{
    public class AudioHandler : GenericSingleton<AudioHandler>
    {
        #region EXPOSED_FIELDS
        [Space(10)]
        [Header("Sounds & Music Database")]
        [SerializeField] private SoundTrackData[] soundsData = null;
        [SerializeField] private MusicTrackData[] musicsData = null;

        [Space(10)]
        [Header("Main References")]
        [SerializeField] private SoundHandlerChannel soundHandlerChannel = null;
        [SerializeField] private MusicHandlerChannel musicHandlerChannel = null;
        [SerializeField] private AudioSourceObject prefabSource = null;
        [Header("Pool Settings")]
        [SerializeField] private int maxSizePool = 10;
        [SerializeField] private Transform holderSoundSources;
        [SerializeField] private Transform holderMusicSources;
        #endregion

        #region PRIVATE_FIELDS
        private bool onFadeIn = false;

        private Dictionary<string, SoundTrackData> soundsDictionary = new Dictionary<string, SoundTrackData>();
        private Dictionary<string, MusicTrackData> musicsDictionary = new Dictionary<string, MusicTrackData>();

        private List<AudioSourceObject> activeSources = new List<AudioSourceObject>();
        private ObjectPool<AudioSourceObject> poolOfSources = null;
        #endregion

        #region UNITY_CALLS
        protected override void Awake()
        {
            base.Awake();
            
            InitializeAudioHandler();
        }

        protected virtual void OnDestroy()
        {
            soundsDictionary.Clear();
            musicsDictionary.Clear();
        }
        #endregion

        #region PUBLIC_METHODS
        [Obsolete]
        public void ResumeAudio()
        {
            /*sourceMusic.Play();
            sourceGameSounds.Play();*/
        }

        [Obsolete]
        public void PauseAudio()
        {
            /*sourceMusic.Pause();
            sourceGameSounds.Pause();*/
        }

        [Obsolete]
        public void FadeOutAudio(float loadingTime, AudioSource source)
        {
            /*IEnumerator FadeOut()
            {
                if(sourceMusic != null)
                {
                    float decreaseSpeed = source.volume / loadingTime;

                    while (source.volume > 0.001f)
                    {
                        source.volume -= decreaseSpeed * Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    source.volume = 0f;
                    yield break;
                }
                else
                {
                    yield break;
                }
            }

            StartCoroutine(FadeOut());*/
        }

        public void FadeInAudio(float loadingTime, float savedTargetVol, AudioSource source)
        {            
            float realTargetVolume = savedTargetVol / 100f;

            IEnumerator FadeIn()
            {
                if (source != null)
                {
                    onFadeIn = true;

                    while (source.volume < realTargetVolume)
                    {
                        source.volume += Time.deltaTime / loadingTime;
                        yield return new WaitForEndOfFrame();
                    }
                    source.volume = realTargetVolume;

                    onFadeIn = false;
                    yield break;
                }
                else
                {
                    yield break;
                }
            }

            StartCoroutine(FadeIn());
        }

        public void ChangeSoundsVolume(float value)
        {
            foreach (SoundTrackData sound in soundsData)
            {
                if(sound != null)
                {
                    sound.volume = value;
                }
            }

            for (int i = 0; i < activeSources.Count; i++)
            {
                if (activeSources[i] != null)
                {
                    if (activeSources[i].transform.parent == holderSoundSources)
                    {
                        activeSources[i].UpdateVolume(value);
                    }
                }
            }
        }

        public void ChangeMusicsVolume(float value)
        {
            foreach (MusicTrackData music in musicsData)
            {
                if (music != null)
                {
                    music.volume = value;
                }
            }

            if(!onFadeIn)
            {
                for (int i = 0; i < activeSources.Count; i++)
                {
                    if (activeSources[i] != null)
                    {
                        if (activeSources[i].transform.parent == holderMusicSources)
                        {
                            activeSources[i].UpdateVolume(value);
                        }
                    }
                }
            }
        }
        #endregion

        #region PROTECTED_METHODS
        protected void InitializeAudioHandler()
        {
            poolOfSources = new ObjectPool<AudioSourceObject>(GenerateAudioSource, GetTrackSource, ReleaseTrackSource, maxSizePool);

            InitializeHandlers();

            FillAudioData();
        }

        protected AudioSourceObject GenerateAudioSource()
        {
            AudioSourceObject trackSource = Instantiate(prefabSource, holderSoundSources.transform);
            trackSource.Get();
            return trackSource;
        }

        protected void GetTrackSource(AudioSourceObject trackSource)
        {
            trackSource.Get();
        }

        protected void ReleaseTrackSource(AudioSourceObject trackSource)
        {
            trackSource.Release();
        }

        protected void InitializeHandlers()
        {
            soundHandlerChannel.OnPlaySound += PlaySound;
            soundHandlerChannel.OnStopSound += StopSound;
            soundHandlerChannel.IsSoundPlaying += IsSoundPlaying;

            musicHandlerChannel.OnPlayMusic += PlayMusic;
            musicHandlerChannel.OnStopMusic += StopMusic;
            musicHandlerChannel.IsMusicPlaying += IsMusicPlaying;
        }

        protected void FillAudioData()
        {
            for (int i = 0; i < soundsData.Length; i++)
            {
                if (soundsData[i] != null)
                {
                    soundsDictionary.Add(soundsData[i].id, soundsData[i]);
                }
            }

            for (int i = 0; i < musicsData.Length; i++)
            {
                if (musicsData[i] != null)
                {
                    musicsDictionary.Add(musicsData[i].id, musicsData[i]);
                }
            }
        }

        protected virtual bool IsSoundPlaying(string id)
        {
            if (!activeSources.Any()) return false;

            return activeSources.Find(source => source.SourceData.id == id);
        }

        protected virtual bool IsMusicPlaying(string id)
        {
            if (!activeSources.Any()) return false;

            return activeSources.Find(source => source.SourceData.id == id);
        }

        protected virtual void PlaySound(string id)
        {
            if(soundsDictionary[id] != null)
            {
                AudioSourceObject newSourceSound = poolOfSources.Get();
                newSourceSound.transform.SetParent(holderSoundSources);

                SoundTrackData soundData = soundsDictionary[id];

                if (!activeSources.Contains(newSourceSound))
                {
                    activeSources.Add(newSourceSound);
                }

                newSourceSound.Configure(soundData, true, onTrackEnded: ()=> 
                {
                    if(soundData.releaseAfterPlayfack)
                    {
                        StopSound(id);
                    }

                    if (soundData.loop)
                    {
                        newSourceSound.Play();
                    }
                });
            }
        }

        protected virtual void StopSound(string id)
        {
            if (soundsDictionary[id] != null)
            {
                AudioSourceObject sourceSound = null;
                for (int i = 0; i < activeSources.Count; i++)
                {
                    if (activeSources[i] != null)
                    {
                        if(activeSources[i].SourceData != null)
                        {
                            if(activeSources[i].SourceData.id == id)
                            {
                                sourceSound = activeSources[i];
                            }
                        }
                    }
                }

                if(sourceSound == null)
                {
                    Debug.LogWarning("The sound with that id is not beign reproduced.");
                    return;
                }

                sourceSound.Stop();

                poolOfSources.Release(sourceSound);
                sourceSound.transform.SetParent(transform);
                activeSources.Remove(sourceSound);
            }
        }

        protected virtual void PlayMusic(string id)
        {
            if (musicsDictionary[id] != null)
            {
                AudioSourceObject newSourceMusic = poolOfSources.Get();
                newSourceMusic.transform.SetParent(holderMusicSources);

                MusicTrackData musicData = musicsDictionary[id];

                AudioSourceObject foundedSource = activeSources.Find(source => source.SourceData.id == id);
                if (!activeSources.Contains(foundedSource))
                {
                    activeSources.Add(foundedSource);
                }

                newSourceMusic.Configure(musicData, true, onTrackEnded: () =>
                {
                    if (!musicData.loop)
                    {
                        AudioSourceObject sourceObj = activeSources.Find(source => source.SourceData.id == id);
                        if(sourceObj)
                        {
                            poolOfSources.Release(sourceObj);
                            sourceObj.transform.SetParent(transform);
                            activeSources.Remove(sourceObj);
                        }
                    }
                    else
                    {
                        newSourceMusic.Play();
                    }
                });
            }
        }

        protected virtual void StopMusic(string id)
        {
            if (musicsDictionary[id] != null)
            {
                AudioSourceObject sourceMusic = activeSources.Find(source=> source.SourceData.id == id);

                if (sourceMusic == null)
                {
                    Debug.LogWarning("The sound with that id is not beign reproduced.");
                    return;
                }

                sourceMusic.Stop();

                poolOfSources.Release(sourceMusic);
                sourceMusic.transform.SetParent(transform);
                activeSources.Remove(sourceMusic);
            }
        }
        #endregion
    }
}