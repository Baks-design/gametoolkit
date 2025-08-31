using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public abstract class CustomMonoBehaviour
        : MonoBehaviour,
            IManagedObject,
            IFixedUpdatable,
            IUpdatable,
            ILateUpdatable
    {
        IUpdateServices updateServices;

        protected Transform Transform { get; private set; }

        protected virtual void Awake() => Transform = transform;

        protected virtual void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        protected virtual void Start() { }

        public virtual void ProcessFixedUpdate(float deltaTime) { }

        public virtual void ProcessUpdate(float deltaTime) { }

        public virtual void ProcessLateUpdate(float deltaTime) { }

        protected virtual void OnDisable()
        {
            if (updateServices == null)
                return;
            updateServices.Unregister(this);
        }
    }
}
