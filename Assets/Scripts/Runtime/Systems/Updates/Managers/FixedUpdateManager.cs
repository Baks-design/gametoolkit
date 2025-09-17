using System.Collections.Generic;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class FixedUpdateManager : MonoBehaviour, IUpdateServices
    {
        readonly List<IFixedUpdatable> fixedUpdatableObjects = new();
        readonly List<IFixedUpdatable> pendingObjects = new();
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

            if (
                obj is IFixedUpdatable fixedUpdatable
                && !fixedUpdatableObjects.Contains(fixedUpdatable)
            )
                fixedUpdatableObjects.Add(fixedUpdatable);
        }

        void FixedUpdate()
        {
            for (currentIndex = fixedUpdatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                fixedUpdatableObjects[currentIndex].ProcessFixedUpdate(Time.deltaTime);

            fixedUpdatableObjects.AddRange(pendingObjects);

            pendingObjects.Clear();
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null)
                return;

            if (obj is IFixedUpdatable fixedUpdatable)
                fixedUpdatableObjects.Remove(fixedUpdatable);

            currentIndex--;
        }
    }
}
