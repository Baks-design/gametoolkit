using System;
using static UnityEngine.InputSystem.InputAction;

namespace GameToolkit.Runtime.Systems.Input
{
    public class UIInputMap : InputSystem_Actions.IUIActions
    {
        public event Action OnCloseMenu;

        public UIInputMap(InputSystem_Actions inputActions) => inputActions.UI.SetCallbacks(this);

        public void OnClosePauseScreen(CallbackContext context)
        {
            if (context.performed)
                OnCloseMenu?.Invoke();
        }

        public void OnCancel(CallbackContext context) { }

        public void OnClick(CallbackContext context) { }

        public void OnMiddleClick(CallbackContext context) { }

        public void OnNavigate(CallbackContext context) { }

        public void OnPoint(CallbackContext context) { }

        public void OnRightClick(CallbackContext context) { }

        public void OnScrollWheel(CallbackContext context) { }

        public void OnSubmit(CallbackContext context) { }

        public void OnTrackedDeviceOrientation(CallbackContext context) { }

        public void OnTrackedDevicePosition(CallbackContext context) { }
    }
}
