using System.Collections.Generic;
using Alchemy.Inspector;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameToolkit.Runtime.Game.Systems.Sound
{
    [DisallowMultipleComponent]
    public class SoundManager : MonoBehaviour, ISoundServices
    {
        [SerializeField, AssetsOnly, Required]
        SoundEmitter soundEmitterPrefab;

        [SerializeField]
        bool collectionCheck = true;

        [SerializeField]
        int defaultCapacity = 10;

        [SerializeField]
        int maxPoolSize = 100;

        [SerializeField]
        int maxSoundInstances = 30;

        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

        public void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }

        void InitializePool() =>
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );

        public SoundBuilder CreateSoundBuilder() => new(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound)
                return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Logging.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get() => soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter) =>
            soundEmitterPool.Release(soundEmitter);

        public void StopAll()
        {
            foreach (var soundEmitter in activeSoundEmitters)
                soundEmitter.Stop();

            FrequentSoundEmitters.Clear();
        }

        SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            if (soundEmitter == null)
                return;
            Destroy(soundEmitter.gameObject);
        }
    }
}
