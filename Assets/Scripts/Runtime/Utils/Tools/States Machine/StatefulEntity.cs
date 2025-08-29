using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    /// <summary>
    /// Awake or Start can be used to declare all states and transitions.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override void Awake() {
    ///     base.Awake();
    ///
    ///     var state = new State1(this);
    ///     var anotherState = new State2(this);
    ///
    ///     At(state, anotherState, () => true);
    ///     At(state, anotherState, myFunc);
    ///     At(state, anotherState, myPredicate);
    ///
    ///     Any(anotherState, () => true);
    ///
    ///     stateMachine.SetState(state);
    /// </code>
    /// </example>
    public abstract class StatefulEntity
        : MonoBehaviour,
            IManagedObject,
            IFixedUpdatable,
            IUpdatable,
            ILateUpdatable
    {
        protected StateMachine stateMachine;
        protected Transform tr;
        IUpdateServices updateServices;

        protected virtual void Awake()
        {
            stateMachine = new StateMachine();
            tr = transform;
        }

        protected virtual void OnEnable()
        {
            ServiceLocator.Global.Get(out updateServices);
            updateServices.Register(this);
        }

        public virtual void ProcessFixedUpdate(float deltaTime) =>
            stateMachine.FixedUpdate(Time.deltaTime);

        public virtual void ProcessUpdate(float deltaTime) => stateMachine.Update(Time.deltaTime);

        public virtual void ProcessLateUpdate(float deltaTime) =>
            stateMachine.LateUpdate(Time.deltaTime);

        protected virtual void OnDisable() => updateServices.Unregister(this);

        protected void At<T>(IState from, IState to, T condition) =>
            stateMachine.AddTransition(from, to, condition);

        protected void Any<T>(IState to, T condition) =>
            stateMachine.AddAnyTransition(to, condition);
    }
}
