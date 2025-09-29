using System;
using System.Threading;
using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CrouchHandler
    {
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly float crouchCamHeight;
        readonly float initCamHeight;
        readonly float crouchHeight;
        readonly float crouchStandHeightDifference;
        Vector3 crouchCenter;
        CancellationTokenSource crouchCancellationTokenSource;

        public CrouchHandler(
            CharacterController controller,
            Transform yawTransform,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData
        )
        {
            this.controller = controller;
            this.yawTransform = yawTransform;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.movementData = movementData;

            initCamHeight = yawTransform.localPosition.y;

            crouchHeight = collisionData.InitHeight * movementConfig.CrouchPercent;
            crouchCenter = (crouchHeight / 2f + controller.skinWidth) * Vector3.up;
            crouchStandHeightDifference = collisionData.InitHeight - crouchHeight;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;

            movementData.CurrentStateHeight = initCamHeight;
        }

        public void HandleCrouch(float deltaTime)
        {
            if (!InputManager.CrouchPressed)
                return;

            if (movementData.IsCrouching && collisionData.HasRoofed)
                return;

            // Cancel previous crouch animation
            crouchCancellationTokenSource?.Cancel();
            crouchCancellationTokenSource = new CancellationTokenSource();

            _ = StartCrouch(deltaTime, crouchCancellationTokenSource.Token);
        }

        async Awaitable StartCrouch(float deltaTime, CancellationToken cancellationToken = default)
        {
            try
            {
                await CrouchAsync(deltaTime, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when crouch is cancelled
            }
        }

        async Awaitable CrouchAsync(float deltaTime, CancellationToken cancellationToken = default)
        {
            movementData.IsDuringCrouchAnimation = true;

            var percent = 0f;
            var speed = 1f / movementConfig.CrouchTransitionDuration;

            var currentHeight = controller.height;
            var currentCenter = controller.center;

            var desiredHeight = movementData.IsCrouching ? collisionData.InitHeight : crouchHeight;
            var desiredCenter = movementData.IsCrouching ? collisionData.InitCenter : crouchCenter;

            var camPos = yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var camDesiredHeight = movementData.IsCrouching ? initCamHeight : crouchCamHeight;

            movementData.IsCrouching = !movementData.IsCrouching;
            movementData.CurrentStateHeight = movementData.IsCrouching
                ? crouchCamHeight
                : initCamHeight;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var smoothPercent = movementConfig.CrouchTransitionCurve.Evaluate(percent);

                controller.height = Mathf.Lerp(currentHeight, desiredHeight, smoothPercent);
                controller.center = Vector3.Lerp(currentCenter, desiredCenter, smoothPercent);

                camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, smoothPercent);
                yawTransform.localPosition = camPos;

                await Awaitable.NextFrameAsync();
            }

            movementData.IsDuringCrouchAnimation = false;
        }
    }
}
