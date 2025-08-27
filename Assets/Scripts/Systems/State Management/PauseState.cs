using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class PauseState : IState
    {
        StateManager stateManager;

        public PauseState(StateManager stateManager) => this.stateManager = stateManager;

        public void OnEnter()
        {
            Time.timeScale = 0f;
            Logging.Log("Enter In Pause State");
        }
    }
}
