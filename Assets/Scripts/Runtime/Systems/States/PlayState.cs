using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class PlayState : IState
    {
        readonly InputManager inputManager;

        public PlayState(InputManager inputManager) => this.inputManager = inputManager;

        public void OnEnter()
        {
            Logging.Log("Enter In Play State");
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            inputManager.EnablePlayerMap();
        }
    }
}
