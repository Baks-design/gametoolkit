using UnityEngine;
using UnityEngine.InputSystem;

namespace GameToolkit.Runtime.Application.Input
{
    public class InputManager : IInputServices, IMovementInput, IUIInput
    {
        InputSystem_Actions inputActions;

        public void Initialize() => inputActions = new InputSystem_Actions();

        #region Services
        public void EnablePlayerInput()
        {
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }

        public void EnableUIInput()
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }

        public void DisableAllInput()
        {
            inputActions.Player.Disable();
            inputActions.UI.Disable();
        }
        #endregion

        #region Player
        public InputAction OpenMenuPressed() => inputActions.Player.OpenPauseScreen;

        public Vector2 GetLook() => inputActions.Player.Look.ReadValue<Vector2>();

        public bool AimPressed() => inputActions.Player.Aim.WasPerformedThisFrame();

        public bool AimReleased() => inputActions.Player.Aim.WasReleasedThisFrame();

        public Vector2 GetMovement() => inputActions.Player.Move.ReadValue<Vector2>();

        public bool HasMovement() => inputActions.Player.Move.ReadValue<Vector2>() != Vector2.zero;

        public bool JumpPressed() => inputActions.Player.Jump.WasPerformedThisFrame();

        public bool CrouchPressed() => inputActions.Player.Crouch.WasPerformedThisFrame();

        public bool SprintPressed() => inputActions.Player.Sprint.WasPressedThisFrame();

        public bool SprintReleased() => inputActions.Player.Sprint.WasReleasedThisFrame();
        #endregion

        #region UI
        public InputAction CloseMenuPressed() => inputActions.UI.ClosePauseScreen;
        #endregion
    }
}
