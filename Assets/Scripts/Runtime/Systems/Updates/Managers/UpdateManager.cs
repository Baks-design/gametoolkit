using System.Collections.Generic;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class UpdateManager : MonoBehaviour, IUpdateServices
    {
        readonly List<IUpdatable> updatableObjects = new();
        readonly List<IUpdatable> pendingObjects = new();
        static int currentIndex;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IUpdateServices>(this);
        }

        public void Register(IManagedObject obj)
        {
            if (obj == null)
                return;

            if (obj is IUpdatable updatable && !updatableObjects.Contains(updatable))
                updatableObjects.Add(updatable);
        }

        void Update()
        {
            for (currentIndex = updatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                updatableObjects[currentIndex].ProcessUpdate(Time.deltaTime);

            updatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null)
                return;

            if (obj is IUpdatable updatable)
                updatableObjects.Remove(updatable);

            currentIndex--;
        }
    }
}
