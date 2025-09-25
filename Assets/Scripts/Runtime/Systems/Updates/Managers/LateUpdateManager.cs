using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class LateUpdateManager : MonoBehaviour, ILateUpdateServices
    {
        readonly List<ILateUpdatable> lateUpdatableObjects = new();
        readonly List<ILateUpdatable> pendingObjects = new();
        int currentIndex;

        public void Initialize() => DontDestroyOnLoad(gameObject);

        public void Register(ILateUpdatable obj) => lateUpdatableObjects.Add(obj);

        void LateUpdate()
        {
            for (currentIndex = lateUpdatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                lateUpdatableObjects[currentIndex].ProcessLateUpdate(Time.deltaTime);

            lateUpdatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public void Unregister(ILateUpdatable obj)
        {
            lateUpdatableObjects.Remove(obj);
            currentIndex--;
        }
    }
}
