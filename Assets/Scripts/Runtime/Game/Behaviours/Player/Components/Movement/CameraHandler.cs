using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CameraHandler
    {
        readonly IMovementInput movementInput;
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerCamera cameraController;
        readonly Transform yawTransform;
        readonly HeadBobHandler headBobHandler;
        readonly RunnningHandler runnningHandler;

        public CameraHandler(
            IMovementInput movementInput,
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            HeadBobHandler headBobHandler,
            PlayerMovementData movementData,
            PlayerCamera cameraController,
            Transform yawTransform,
            RunnningHandler runnningHandler
        )
        {
            this.movementInput = movementInput;
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.headBobHandler = headBobHandler;
            this.movementData = movementData;
            this.cameraController = cameraController;
            this.yawTransform = yawTransform;
            this.runnningHandler = runnningHandler;
        }

        public void RotateTowardsCamera(float deltaTime) =>
            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation,
                yawTransform.rotation,
                deltaTime * movementConfig.SmoothRotateSpeed
            );

        public void HandleHeadBob(float deltaTime)
        {
            var shouldBob = movementInput.HasMovement() && !collisionData.HasObstructed;
            var canBob = shouldBob && !movementData.IsDuringCrouchAnimation;

            if (canBob)
            {
                var canRun = movementData.IsRunning && runnningHandler.CanRun();
                headBobHandler.ScrollHeadBob(
                    canRun,
                    movementData.IsCrouching,
                    movementInput.GetMovement(),
                    deltaTime
                );

                UpdateHeadPosition(
                    deltaTime,
                    (Vector3.up * movementData.CurrentStateHeight) + movementData.FinalOffset
                );
            }
            else
            {
                ResetHeadBobState();
                UpdateHeadPosition(deltaTime, new Vector3(0f, movementData.CurrentStateHeight, 0f));
            }
        }

        public void HandleCameraSway(float deltaTime) =>
            cameraController.SwayHandler(
                movementData.SmoothInputVector,
                movementInput.GetMovement().x,
                deltaTime
            );

        public void HandleRunFOV(float deltaTime)
        {
            var canStartRun =
                movementInput.HasMovement()
                && !collisionData.HasObstructed
                && runnningHandler.CanRun();

            var shouldStartRun =
                canStartRun
                && (
                    movementInput.SprintPressed()
                    || (movementData.IsRunning && !movementData.IsDuringRunAnimation)
                );

            var shouldStopRun =
                movementInput.SprintReleased()
                || !movementInput.HasMovement()
                || collisionData.HasObstructed;

            if (shouldStartRun && !movementData.IsDuringRunAnimation)
            {
                movementData.IsDuringRunAnimation = true;
                cameraController.RunFOVHandler(false, deltaTime);
            }
            else if (shouldStopRun && movementData.IsDuringRunAnimation)
            {
                movementData.IsDuringRunAnimation = false;
                cameraController.RunFOVHandler(true, deltaTime);
            }
        }

        // Helper methods
        void UpdateHeadPosition(float deltaTime, Vector3 targetPosition)
        {
            if (movementData.IsDuringCrouchAnimation)
                return;

            yawTransform.localPosition = Vector3.Lerp(
                yawTransform.localPosition,
                targetPosition,
                deltaTime * movementConfig.SmoothHeadBobSpeed
            );
        }

        void ResetHeadBobState()
        {
            if (!movementData.Resetted)
                headBobHandler.ResetHeadBob();
        }
    }
}
