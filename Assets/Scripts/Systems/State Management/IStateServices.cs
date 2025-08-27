using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public interface IStateServices
    {
        public IState CurrentState { get; }

        void ChangeState(IState state);
    }
}
