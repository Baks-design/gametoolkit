using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public abstract class StatefulEntity : MonoBehaviour
    {
        protected Transform Transform;
        protected StateMachine stateMachine;

        protected virtual void Awake()
        {
            Transform = transform;
            stateMachine = new StateMachine();
        }

        protected virtual void Start() { }

        protected void At<T>(IState from, IState to, T condition) =>
            stateMachine.AddTransition(from, to, condition);

        protected void Any<T>(IState to, T condition) =>
            stateMachine.AddAnyTransition(to, condition);
    }
}
