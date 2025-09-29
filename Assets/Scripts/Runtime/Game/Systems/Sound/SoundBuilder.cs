using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Systems.Sound
{
    public class SoundBuilder
    {
        readonly SoundManager soundManager;
        Vector3 position = Vector3.zero;
        Transform tr;
        bool randomPitch;
        bool setVolume;

        public SoundBuilder(SoundManager soundManager) => this.soundManager = soundManager;

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            randomPitch = true;
            return this;
        }

        public SoundBuilder WithSetVolume(float volume = 1f)
        {
            setVolume = true;
            return this;
        }

        public SoundBuilder WithGameObjectAsParent(Transform transform)
        {
            tr = transform;
            return this;
        }

        public void Play(SoundData soundData)
        {
            if (soundData == null)
            {
                Logging.LogError("SoundData is null");
                return;
            }

            if (!soundManager.CanPlaySound(soundData))
                return;

            var soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            if (tr == null)
            {
                soundEmitter.transform.parent = soundManager.transform;
                soundEmitter.transform.position = position;
            }
            else
            {
                soundEmitter.transform.parent = tr;
                soundEmitter.transform.localPosition = Vector3.zero;
            }

            if (randomPitch)
                soundEmitter.WithRandomPitch();

            if (setVolume)
                soundEmitter.WithSetVolume();

            if (soundData.frequentSound)
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);

            soundEmitter.Play();
        }
    }
}
