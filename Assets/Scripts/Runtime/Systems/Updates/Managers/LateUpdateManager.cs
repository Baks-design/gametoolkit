using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class LateUpdateManager : MonoBehaviour
    {
        static List<ILateUpdatable> lateUpdatableObjects = new();
        static List<ILateUpdatable> pendingObjects = new();
        static int currentIndex;

        void Awake() => DontDestroyOnLoad(gameObject);

        public static void Register(ILateUpdatable obj) => lateUpdatableObjects.Add(obj);

        void LateUpdate()
        {
            for (currentIndex = lateUpdatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                lateUpdatableObjects[currentIndex].ProcessLateUpdate(Time.deltaTime);

            lateUpdatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public static void Unregister(ILateUpdatable obj)
        {
            lateUpdatableObjects.Remove(obj);
            currentIndex--;
        }
    }
}
