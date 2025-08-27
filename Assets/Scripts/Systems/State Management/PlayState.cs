using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class PlayState : IState
    {
        StateManager stateManager;

        public PlayState(StateManager stateManager) => this.stateManager = stateManager;

        public void OnEnter()
        {
            Time.timeScale = 1f;
            Logging.Log("Enter In Play State");
        }
    }
}
