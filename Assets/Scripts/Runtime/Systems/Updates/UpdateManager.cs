using System.Collections.Generic;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public class UpdateManager : MonoBehaviour, IUpdateServices
    {
        readonly List<IFixedUpdatable> fixedUpdatableObjects = new();
        readonly List<IUpdatable> updatableObjects = new();
        readonly List<ILateUpdatable> lateUpdatableObjects = new();

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

            if (obj is IUpdatable updatable && !updatableObjects.Contains(updatable))
                updatableObjects.Add(updatable);

            if (
                obj is ILateUpdatable lateUpdatable
                && !lateUpdatableObjects.Contains(lateUpdatable)
            )
                lateUpdatableObjects.Add(lateUpdatable);
        }

        void FixedUpdate()
        {
            foreach (var obj in fixedUpdatableObjects.ToArray())
                obj?.ProcessFixedUpdate(Time.deltaTime);
        }

        void Update()
        {
            foreach (var obj in updatableObjects.ToArray())
                obj?.ProcessUpdate(Time.deltaTime);
        }

        void LateUpdate()
        {
            foreach (var obj in lateUpdatableObjects.ToArray())
                obj?.ProcessLateUpdate(Time.deltaTime);
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null)
                return;

            if (obj is IFixedUpdatable fixedUpdatable)
                fixedUpdatableObjects.Remove(fixedUpdatable);
            if (obj is IUpdatable updatable)
                updatableObjects.Remove(updatable);
            if (obj is ILateUpdatable lateUpdatable)
                lateUpdatableObjects.Remove(lateUpdatable);
        }
    }
}
