using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class FixedUpdateManager : MonoBehaviour
    {
        static List<IFixedUpdatable> fixedUpdatableObjects = new();
        static List<IFixedUpdatable> pendingObjects = new();
        static int currentIndex;

        void Awake() => DontDestroyOnLoad(gameObject);

        public static void Register(IFixedUpdatable obj) => fixedUpdatableObjects.Add(obj);

        void FixedUpdate()
        {
            for (currentIndex = fixedUpdatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                fixedUpdatableObjects[currentIndex].ProcessFixedUpdate(Time.deltaTime);

            fixedUpdatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public static void Unregister(IFixedUpdatable obj)
        {
            fixedUpdatableObjects.Remove(obj);
            currentIndex--;
        }
    }
}
