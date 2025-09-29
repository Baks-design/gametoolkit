using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Application.States
{
    public class GameplayState : IState
    {
        public void OnEnter()
        {
            Logging.Log("Enter in GameplayState State");
            GameSystem.SetCursor(true);
            GameSystem.SetTimeScale(1f);
            InputManager.EnablePlayerInput();
        }
    }
}
