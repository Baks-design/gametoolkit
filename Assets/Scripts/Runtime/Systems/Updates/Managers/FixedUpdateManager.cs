using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class FixedUpdateManager : MonoBehaviour, IFixedUpdateServices
    {
        readonly List<IFixedUpdatable> fixedUpdatableObjects = new();
        readonly List<IFixedUpdatable> pendingObjects = new();
        int currentIndex;

        public void Initialize() => DontDestroyOnLoad(gameObject);

        public void Register(IFixedUpdatable obj) => fixedUpdatableObjects.Add(obj);

        void FixedUpdate()
        {
            for (currentIndex = fixedUpdatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                fixedUpdatableObjects[currentIndex].ProcessFixedUpdate(Time.deltaTime);

            fixedUpdatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public void Unregister(IFixedUpdatable obj)
        {
            fixedUpdatableObjects.Remove(obj);
            currentIndex--;
        }
    }
}
