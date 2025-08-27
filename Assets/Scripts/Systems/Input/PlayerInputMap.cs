using System;
using static UnityEngine.InputSystem.InputAction;

namespace GameToolkit.Runtime.Systems.Input
{
    public class PlayerInputMap : InputSystem_Actions.IPlayerActions
    {
        public event Action OnOpenMenu;

        public PlayerInputMap(InputSystem_Actions inputActions) =>
            inputActions.Player.SetCallbacks(this);

        public void OnOpenPauseScreen(CallbackContext context)
        {
            if (context.performed)
                OnOpenMenu?.Invoke();
        }
    }
}
