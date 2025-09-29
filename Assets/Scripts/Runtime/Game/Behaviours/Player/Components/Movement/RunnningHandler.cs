using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class RunnningHandler
    {
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerMovementData movementData;

        public RunnningHandler(
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerMovementData movementData
        )
        {
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.movementData = movementData;
        }

        public void HandleRun()
        {
            if (InputManager.SprintPressed)
                movementData.IsRunning = true;
            if (InputManager.SprintReleased)
                movementData.IsRunning = false;

            //Logging.Log($"movementData.IsRunning: {movementData.IsRunning}");
        }

        public bool CanRun()
        {
            var normalizedDir = Vector3.zero;
            if (movementData.SmoothFinalMoveDir != Vector3.zero)
                normalizedDir = movementData.SmoothFinalMoveDir.normalized;
            var dot = Vector3.Dot(controller.transform.forward, normalizedDir);
            return dot >= movementConfig.CanRunThreshold && !movementData.IsCrouching;
        }
    }
}
