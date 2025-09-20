using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class UpdateManager : MonoBehaviour
    {
        static List<IUpdatable> updatableObjects = new();
        static List<IUpdatable> pendingObjects = new();
        static int currentIndex;

        void Awake() => DontDestroyOnLoad(gameObject);

        public static void Register(IUpdatable obj) => updatableObjects.Add(obj);

        void Update()
        {
            for (currentIndex = updatableObjects.Count - 1; currentIndex >= 0; currentIndex--)
                updatableObjects[currentIndex].ProcessUpdate(Time.deltaTime);

            updatableObjects.AddRange(pendingObjects);
            pendingObjects.Clear();
        }

        public static void Unregister(IUpdatable obj)
        {
            updatableObjects.Remove(obj);
            currentIndex--;
        }
    }
}
