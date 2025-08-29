using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Audio
{
    public class SoundBuilder
    {
        readonly SoundManager soundManager;
        Vector3 position = Vector3.zero;
        Transform tr;
        bool randomPitch;

        public SoundBuilder(SoundManager soundManager) => this.soundManager = soundManager;

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            randomPitch = true;
            return this;
        }

        public SoundBuilder WithGameObjectAsParent(Transform transform)
        {
            this.tr = transform;
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

            if (soundData.frequentSound)
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);

            soundEmitter.Play();
        }
    }
}
