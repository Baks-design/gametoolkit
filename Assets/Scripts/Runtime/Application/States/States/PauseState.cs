using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Application.States
{
    public class PauseState : IState
    {
        public void OnEnter()
        {
            Logging.Log("Enter in PauseState State");
            GameSystem.SetCursor(false);
            GameSystem.SetTimeScale(0f);
            InputManager.EnableUIInput();
        }
    }
}
