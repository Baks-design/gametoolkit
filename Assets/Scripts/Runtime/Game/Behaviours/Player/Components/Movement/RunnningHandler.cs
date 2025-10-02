using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class RunnningHandler
    {
        readonly IMovementInput movementInput;
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerMovementData movementData;

        public RunnningHandler(
            IMovementInput movementInput,
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerMovementData movementData
        )
        {
            this.movementInput = movementInput;
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.movementData = movementData;
        }

        public void HandleRun()
        {
            if (movementInput.SprintPressed())
                movementData.IsRunning = true;
            else if (movementInput.SprintReleased())
                movementData.IsRunning = false;
        }

        public bool CanRun()
        {
            if (movementData.IsCrouching || movementData.SmoothFinalMoveDir == Vector3.zero)
                return false;

            var normalizedDir = movementData.SmoothFinalMoveDir.normalized;
            var dot = Vector3.Dot(controller.transform.forward, normalizedDir);
            return dot >= movementConfig.CanRunThreshold;
        }
    }
}
