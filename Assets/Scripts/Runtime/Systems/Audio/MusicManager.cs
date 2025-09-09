using System.Collections.Generic;
using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Extensions;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;
using UnityEngine.Audio;

namespace GameToolkit.Runtime.Systems.Audio
{
    [DisallowMultipleComponent]
    public class MusicManager : CustomMonoBehaviour, IMusicServices
    {
        [SerializeField]
        AudioMixerGroup musicMixerGroup;

        [SerializeField]
        List<AudioClip> initialPlaylist;

        AudioSource current;
        AudioSource previous;
        readonly Queue<AudioClip> playlist = new();
        float fading;
        const float crossFadeTime = 1f;

        protected override void Awake()
        {
            Setup();
            FillSongs();
        }

        void Setup()
        {
            ServiceLocator.Global.Register<IMusicServices>(this);
            DontDestroyOnLoad(gameObject);
        }

        void FillSongs()
        {
            if (initialPlaylist == null)
                return;

            foreach (var clip in initialPlaylist)
                AddToPlaylist(clip);
        }

        public void AddToPlaylist(AudioClip clip)
        {
            playlist.Enqueue(clip);
            if (current == null && previous == null)
                PlayNextTrack();
        }

        public void Clear() => playlist.Clear();

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out AudioClip nextTrack))
                Play(nextTrack);
        }

        public void Play(AudioClip clip)
        {
            if (current && current.clip == clip)
                return;

            if (previous)
            {
                Destroy(previous);
                previous = null;
            }

            previous = current;

            current = gameObject.GetOrAdd<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup; // Set mixer group
            current.loop = false; // For playlist functionality, we want tracks to play once
            current.volume = 0f;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }

        public override void ProcessUpdate(float deltaTime)
        {
            HandleCrossFade(deltaTime);

            if (current && !current.isPlaying && playlist.Count > 0)
                PlayNextTrack();
        }

        void HandleCrossFade(float deltaTime)
        {
            if (fading <= 0f)
                return;

            fading += deltaTime;

            var fraction =
                crossFadeTime > Mathf.Epsilon ? Mathf.Clamp01(fading / crossFadeTime) : 1f;
            var logFraction = fraction.ToLogarithmicFraction();

            if (previous)
                previous.volume = 1f - logFraction;
            if (current)
                current.volume = logFraction;

            if (fraction >= 1f)
            {
                fading = 0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }
    }
}
