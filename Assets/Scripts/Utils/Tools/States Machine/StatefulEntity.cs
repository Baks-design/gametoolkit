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
    public abstract class StatefulEntity : MonoBehaviour, IManagedObject
    {
        protected StateMachine stateMachine;
        protected Transform tr;
        IUpdateServices updateServices;

        protected virtual void Awake()
        {
            stateMachine = new StateMachine();
            tr = transform;
        }

        protected virtual void Start()
        {
            ServiceLocator.Global.Get(out updateServices);
            updateServices.Register(this);
        }

        protected virtual void FixedUpdate() => stateMachine.FixedUpdate(Time.deltaTime);

        protected virtual void Update() => stateMachine.Update(Time.deltaTime);

        protected virtual void LatedUpdate() => stateMachine.LateUpdate(Time.deltaTime);

        protected virtual void OnDisable() => updateServices.Unregister(this);

        protected void At<T>(IState from, IState to, T condition) =>
            stateMachine.AddTransition(from, to, condition);

        protected void Any<T>(IState to, T condition) =>
            stateMachine.AddAnyTransition(to, condition);
    }
}
