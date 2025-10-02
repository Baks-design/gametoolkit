using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CrouchHandler
    {
        readonly IMovementInput movementInput;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly IPlayerCollision collision;
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly float crouchCamHeight;
        readonly float initCamHeight;
        readonly float crouchHeight;
        readonly float crouchStandHeightDifference;
        Vector3 crouchCenter;
        CancellationTokenSource crouchCancellationTokenSource;

        public CrouchHandler(
            IMovementInput movementInput,
            CharacterController controller,
            Transform yawTransform,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            IPlayerCollision collision
        )
        {
            this.movementInput = movementInput;
            this.controller = controller;
            this.yawTransform = yawTransform;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.collision = collision;

            initCamHeight = yawTransform.localPosition.y;

            crouchHeight = collisionData.InitHeight * movementConfig.CrouchPercent;
            crouchCenter = (crouchHeight / 2f + controller.skinWidth) * Vector3.up;
            crouchStandHeightDifference = collisionData.InitHeight - crouchHeight;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;

            movementData.CurrentStateHeight = initCamHeight;
        }

        public void HandleCrouch(float deltaTime)
        {
            var canCrouch =
                movementInput.CrouchPressed()
                && !movementData.IsCrouching
                && !collision.RoofCheckHandler();

            if (!canCrouch)
                return;

            // Cancel previous crouch animation
            crouchCancellationTokenSource?.Cancel();
            crouchCancellationTokenSource = new CancellationTokenSource();

            _ = StartCrouch(deltaTime, crouchCancellationTokenSource.Token);
        }

        async UniTaskVoid StartCrouch(
            float deltaTime,
            CancellationToken cancellationToken = default
        )
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

        async UniTask CrouchAsync(float deltaTime, CancellationToken cancellationToken = default)
        {
            movementData.IsDuringCrouchAnimation = true;

            // Pre-calculate target values
            var wasCrouching = movementData.IsCrouching;
            movementData.IsCrouching = !wasCrouching;

            var targetHeight = wasCrouching ? collisionData.InitHeight : crouchHeight;
            var targetCenter = wasCrouching ? collisionData.InitCenter : crouchCenter;
            var targetCamHeight = wasCrouching ? initCamHeight : crouchCamHeight;

            movementData.CurrentStateHeight = wasCrouching ? initCamHeight : crouchCamHeight;

            // Cache initial values
            var initialHeight = controller.height;
            var initialCenter = controller.center;
            var initialCamPos = yawTransform.localPosition;
            var initialCamHeight = initialCamPos.y;

            var percent = 0f;
            var speed = 1f / movementConfig.CrouchTransitionDuration;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var smoothPercent = movementConfig.CrouchTransitionCurve.Evaluate(percent);

                // Apply interpolation
                controller.height = Mathf.Lerp(initialHeight, targetHeight, smoothPercent);
                controller.center = Vector3.Lerp(initialCenter, targetCenter, smoothPercent);

                // Reuse vector to avoid allocation
                initialCamPos.y = Mathf.Lerp(initialCamHeight, targetCamHeight, smoothPercent);
                yawTransform.localPosition = initialCamPos;

                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }

            movementData.IsDuringCrouchAnimation = false;
        }
    }
}
