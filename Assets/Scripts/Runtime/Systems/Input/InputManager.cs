using System;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Input
{
    public static class InputManager
    {
        static InputSystem_Actions inputActions;

        public static bool IsInitialized { get; private set; }

        public static event Action<Vector2> OnLookPerformed;
        public static event Action OnLookCanceled;
        public static event Action OnAimStarted;
        public static event Action OnAimCanceled;
        public static event Action<Vector2> OnMovePerformed;
        public static event Action OnMoveCanceled;
        public static event Action OnJumpPerformed;
        public static event Action OnCrouchPerformed;
        public static event Action OnSprintStarted;
        public static event Action OnSprintCanceled;
        public static event Action OnPausePerformed;

        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (IsInitialized)
                return;

            inputActions = new InputSystem_Actions();
            EnablePlayerInput();
            SubscribeToEvents();

            IsInitialized = true;
            //Logging.Log("InputManager initialized successfully.");
        }

        static void SubscribeToEvents()
        {
            // Look
            inputActions.Player.Look.performed += ctx =>
                OnLookPerformed?.Invoke(ctx.ReadValue<Vector2>());
            inputActions.Player.Look.canceled += ctx => OnLookCanceled?.Invoke();
            // Aim
            inputActions.Player.Aim.started += ctx => OnAimStarted?.Invoke();
            inputActions.Player.Aim.canceled += ctx => OnAimCanceled?.Invoke();
            // Movement
            inputActions.Player.Move.performed += ctx =>
                OnMovePerformed?.Invoke(ctx.ReadValue<Vector2>());
            inputActions.Player.Move.canceled += ctx => OnMoveCanceled?.Invoke();
            // Jump
            inputActions.Player.Jump.performed += ctx => OnJumpPerformed?.Invoke();
            // Crouch
            inputActions.Player.Crouch.performed += ctx => OnCrouchPerformed?.Invoke();
            // Sprint
            inputActions.Player.Sprint.performed += ctx => OnSprintStarted?.Invoke();
            inputActions.Player.Sprint.canceled += ctx => OnSprintCanceled?.Invoke();
            // Interactions
            inputActions.Player.OpenPauseScreen.performed += ctx => OnPausePerformed?.Invoke();
        }
        #endregion

        #region Input State Getters
        public static Vector2 GetLook => inputActions.Player.Look.ReadValue<Vector2>();
        public static bool HasLook
        {
            get
            {
                var value = inputActions.Player.Look.ReadValue<Vector2>();
                return Mathf.Abs(value.x) > 0.1f || Mathf.Abs(value.y) > 0.1f;
            }
        }
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

        #region Cleanup
        public static void Cleanup()
        {
            if (!IsInitialized)
                return;

            UnsubscribeFromEvents();
            inputActions?.Dispose();
            IsInitialized = false;
        }

        static void UnsubscribeFromEvents()
        {
            OnLookPerformed = null;
            OnLookCanceled = null;
            OnAimStarted = null;
            OnAimCanceled = null;
            OnMovePerformed = null;
            OnMoveCanceled = null;
            OnJumpPerformed = null;
            OnCrouchPerformed = null;
            OnSprintStarted = null;
            OnSprintCanceled = null;
            OnPausePerformed = null;
        }
        #endregion
    }
}
