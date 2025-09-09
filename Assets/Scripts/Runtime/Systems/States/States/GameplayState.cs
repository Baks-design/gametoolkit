using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class GameplayState : IState
    {
        public GameplayState() { }

        public void OnEnter()
        {
            Logging.Log("Enter in GameplayState State");
            GameSystem.SetCursor(true);
            GameSystem.SetTimeScale(1f);
            InputManager.EnablePlayerInput();
        }
    }
}
