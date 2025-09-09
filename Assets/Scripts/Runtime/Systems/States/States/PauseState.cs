using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class PauseState : IState
    {
        public PauseState() { }

        public void OnEnter()
        {
            Logging.Log("Enter in PauseState State");
            GameSystem.SetCursor(false);
            GameSystem.SetTimeScale(0f);
            InputManager.EnableUIInput();
        }
    }
}
