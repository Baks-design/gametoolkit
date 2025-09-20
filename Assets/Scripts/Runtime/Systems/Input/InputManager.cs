using UnityEngine;

namespace GameToolkit.Runtime.Systems.Input
{
    public static class InputManager
    {
        static InputSystem_Actions inputActions;

        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            inputActions = new InputSystem_Actions();
            EnablePlayerInput();
            //Logging.Log("InputManager initialized successfully.");
        }
        #endregion

        #region Input State Getters
        public static Vector2 GetLook => inputActions.Player.Look.ReadValue<Vector2>();
        public static bool AimPressed => inputActions.Player.Aim.WasPerformedThisFrame();
        public static bool AimReleased => inputActions.Player.Aim.WasReleasedThisFrame();
        public static Vector2 GetMovement => inputActions.Player.Move.ReadValue<Vector2>();
        public static bool HasMovement
        {
            get
            {
                var value = inputActions.Player.Move.ReadValue<Vector2>();
                return Mathf.Abs(value.x) > 0.1f || Mathf.Abs(value.y) > 0.1f;
            }
        }
        public static bool JumpPressed => inputActions.Player.Jump.WasPerformedThisFrame();
        public static bool CrouchPressed => inputActions.Player.Crouch.WasPerformedThisFrame();
        public static bool SprintPressed => inputActions.Player.Sprint.WasPressedThisFrame();
        public static bool SprintReleased => inputActions.Player.Sprint.WasReleasedThisFrame();
        #endregion

        #region Enable/Disable Methods
        public static void EnablePlayerInput()
        {
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }

        public static void EnableUIInput()
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }

        public static void DisableAllInput()
        {
            inputActions.Player.Disable();
            inputActions.UI.Disable();
        }
        #endregion
    }
}
