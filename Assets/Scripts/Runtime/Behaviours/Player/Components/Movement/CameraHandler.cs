using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CameraHandler
    {
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerCameraController cameraController;
        readonly Transform yawTransform;
        readonly HeadBobHandler headBobHandler;
        readonly RunnningHandler runnningHandler;

        public CameraHandler(
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            HeadBobHandler headBobHandler,
            PlayerMovementData movementData,
            PlayerCameraController cameraController,
            Transform yawTransform,
            RunnningHandler runnningHandler
        )
        {
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.headBobHandler = headBobHandler;
            this.movementData = movementData;
            this.cameraController = cameraController;
            this.yawTransform = yawTransform;
            this.runnningHandler = runnningHandler;
        }

        public void RotateTowardsCamera(float deltaTime)
        {
            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation,
                yawTransform.rotation,
                deltaTime * movementConfig.SmoothRotateSpeed
            );

            //Logging.Log($"Rotation: {controller.transform.rotation}");
        }

        public void HandleHeadBob(float deltaTime)
        {
            if (InputManager.HasMovement && !collisionData.HasObstructed)
            {
                // we want to make our head bob only if we are moving and not during crouch routine
                if (!movementData.IsDuringCrouchAnimation)
                {
                    headBobHandler.ScrollHeadBob(
                        movementData.IsRunning && runnningHandler.CanRun(),
                        movementData.IsCrouching,
                        InputManager.GetMovement,
                        deltaTime
                    );
                    yawTransform.localPosition = Vector3.Lerp(
                        yawTransform.localPosition,
                        (Vector3.up * movementData.CurrentStateHeight) + movementData.FinalOffset,
                        deltaTime * movementConfig.SmoothHeadBobSpeed
                    );
                }
            }
            else // if we are not moving or we are not grounded
            {
                if (!movementData.Resetted)
                    headBobHandler.ResetHeadBob();

                // we want to reset our head bob only if we are standing still and not during crouch routine
                if (!movementData.IsDuringCrouchAnimation)
                    yawTransform.localPosition = Vector3.Lerp(
                        yawTransform.localPosition,
                        new Vector3(0f, movementData.CurrentStateHeight, 0f),
                        deltaTime * movementConfig.SmoothHeadBobSpeed
                    );
            }
        }

        public void HandleCameraSway(float deltaTime) =>
            cameraController.HandleSway(
                movementData.SmoothInputVector,
                InputManager.GetMovement.x,
                deltaTime
            );

        public void HandleRunFOV(float deltaTime)
        {
            if (InputManager.HasMovement && !collisionData.HasObstructed)
            {
                if (InputManager.SprintPressed && runnningHandler.CanRun())
                {
                    movementData.IsDuringRunAnimation = true;
                    cameraController.ChangeRunFOV(false, deltaTime);
                }

                if (
                    movementData.IsRunning
                    && runnningHandler.CanRun()
                    && !movementData.IsDuringRunAnimation
                )
                {
                    movementData.IsDuringRunAnimation = true;
                    cameraController.ChangeRunFOV(false, deltaTime);
                }
            }

            if (
                InputManager.SprintReleased
                || !InputManager.HasMovement
                || collisionData.HasObstructed
            )
            {
                if (movementData.IsDuringRunAnimation)
                {
                    movementData.IsDuringRunAnimation = false;
                    cameraController.ChangeRunFOV(true, deltaTime);
                }
            }
        }
    }
}
