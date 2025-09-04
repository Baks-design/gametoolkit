using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerInputController : CustomMonoBehaviour
    {
        PlayerInputData inputData;

        public override void ProcessUpdate(float deltaTime)
        {
            base.ProcessUpdate(deltaTime);
            UpdateInput();
        }

        void UpdateInput()
        {
            inputData.LookInput = Mouse.current.delta.ReadValue();

            inputData.ZoomPressed = Mouse.current.rightButton.isPressed;

            inputData.LookInput = Mouse.current.delta.ReadValue();

            if (Keyboard.current.wKey.isPressed)
                inputData.MoveInput.y = 1f;
            else if (Keyboard.current.sKey.isPressed)
                inputData.MoveInput.y = -1f;
            else if (Keyboard.current.dKey.isPressed)
                inputData.MoveInput.x = 1f;
            else if (Keyboard.current.aKey.isPressed)
                inputData.MoveInput.x = -1f;
            else
                inputData.MoveInput = Vector2.zero;

            inputData.HasMoveInput = inputData.MoveInput != Vector2.zero;

            inputData.IsRunning = Keyboard.current.leftShiftKey.isPressed;

            inputData.JumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        }
    }
}
