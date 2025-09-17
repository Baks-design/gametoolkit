using System.Collections.Generic;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class LateUpdateManager : MonoBehaviour, IUpdateServices
    {
        readonly List<ILateUpdatable> lateUpdatableObjects = new();
        readonly List<ILateUpdatable> pendingObjects = new();
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
                obj is ILateUpdatable lateUpdatable
                && !lateUpdatableObjects.Contains(lateUpdatable)
            )
                lateUpdatableObjects.Add(lateUpdatable);
        }

        void LateUpdate()
        {
            for (currentIndex = lateUpdatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                lateUpdatableObjects[currentIndex].ProcessLateUpdate(Time.deltaTime);

            lateUpdatableObjects.AddRange(pendingObjects);

            pendingObjects.Clear();
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null)
                return;

            if (obj is ILateUpdatable lateUpdatable)
                lateUpdatableObjects.Remove(lateUpdatable);

            currentIndex--;
        }
    }
}
