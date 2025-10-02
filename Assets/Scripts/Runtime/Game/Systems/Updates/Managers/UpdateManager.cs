using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Systems.Update
{
    public class UpdateManager : MonoBehaviour, IUpdateServices
    {
        readonly List<IUpdatable> updatableObjects = new();
        readonly List<IUpdatable> pendingObjects = new();
        int currentIndex;

        public void Initialize() => DontDestroyOnLoad(gameObject);

        public void Register(IUpdatable obj) => updatableObjects.Add(obj);

        void Update()
        {
            for (currentIndex = updatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                updatableObjects[currentIndex].ProcessUpdate(Time.deltaTime, Time.time);

            updatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public void Unregister(IUpdatable obj)
        {
            updatableObjects.Remove(obj);
            currentIndex--;
        }
    }
}
