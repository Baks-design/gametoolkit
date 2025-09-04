using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public abstract class StatefulEntity
        : MonoBehaviour,
            IManagedObject,
            IFixedUpdatable,
            IUpdatable,
            ILateUpdatable
    {
        protected Transform Transform;
        protected StateMachine stateMachine;
        IUpdateServices updateServices;

        protected virtual void Awake()
        {
            Transform = transform;
            stateMachine = new StateMachine();
        }

        protected virtual void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        protected virtual void Start() { }

        public virtual void ProcessFixedUpdate(float deltaTime) =>
            stateMachine.FixedUpdate(deltaTime);

        public virtual void ProcessUpdate(float deltaTime) => stateMachine.Update(deltaTime);

        public virtual void ProcessLateUpdate(float deltaTime) =>
            stateMachine.LateUpdate(deltaTime);

        protected virtual void OnDisable()
        {
            if (updateServices == null)
                return;
            updateServices.Unregister(this);
        }

        protected void At<T>(IState from, IState to, T condition) =>
            stateMachine.AddTransition(from, to, condition);

        protected void Any<T>(IState to, T condition) =>
            stateMachine.AddAnyTransition(to, condition);
    }
}
