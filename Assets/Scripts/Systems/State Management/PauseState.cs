using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class PauseState : IState
    {
        InputManager inputManager;

        public PauseState(InputManager inputManager) => this.inputManager = inputManager;

        public void OnEnter()
        {
            Logging.Log("Enter In Pause State");
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            inputManager.EnableUIMap();
        }
    }
}
