using System.Collections.Generic;
using GameToolkit.Runtime.Utils.Extensions;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Systems.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;
        Awaitable playbackAwaitable;
        ISoundServices soundServices;

        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        void OnEnable() => ServiceLocator.Global.Get(out soundServices);

        public void Initialize(SoundData data)
        {
            Data = data;
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;
            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;
            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;
            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;
            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;
            audioSource.rolloffMode = data.rolloffMode;
        }

        public void Play()
        {
            if (playbackAwaitable != null && !playbackAwaitable.IsCompleted)
                playbackAwaitable.Cancel();

            audioSource.Play();
            playbackAwaitable = WaitForSoundToEndAsync();
        }

        async Awaitable WaitForSoundToEndAsync()
        {
            while (audioSource.isPlaying)
                await Awaitable.NextFrameAsync();

            Stop();
        }

        public void Stop()
        {
            // Cancel any ongoing playback awaitable
            if (playbackAwaitable != null && !playbackAwaitable.IsCompleted)
            {
                playbackAwaitable.Cancel();
                playbackAwaitable = null;
            }

            audioSource.Stop();
            soundServices.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) =>
            audioSource.pitch += Random.Range(min, max);

        public void WithSetVolume(float volume = 1f) => audioSource.volume = volume;
    }
}
