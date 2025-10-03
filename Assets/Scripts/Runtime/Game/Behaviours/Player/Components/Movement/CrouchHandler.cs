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
        readonly Vector3 initCenter;
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly float initHeight;
        readonly float crouchCamHeight;
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

            crouchHeight = collisionData.InitHeight * movementConfig.CrouchPercent;
            crouchCenter = (crouchHeight / 2f + controller.skinWidth) * Vector3.up;
            crouchStandHeightDifference = collisionData.InitHeight - crouchHeight;
            crouchCamHeight = movementData.InitCamHeight - crouchStandHeightDifference;
        }

        public void HandleCrouch(float deltaTime)
        {
            var canCrouch =
                movementInput.CrouchPressed()
                && !movementData.IsCrouching
                && !collision.RoofCheckHandler();
            if (!canCrouch)
                return;

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
            catch (OperationCanceledException) { }
        }

        async UniTask CrouchAsync(float deltaTime, CancellationToken cancellationToken = default)
        {
            movementData.IsDuringCrouchAnimation = true;

            var wasCrouching = movementData.IsCrouching;

            var targetHeight = wasCrouching ? collisionData.InitHeight : crouchHeight;
            var targetCenter = wasCrouching ? collisionData.InitCenter : crouchCenter;
            var camPos = yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var targetCamHeight = wasCrouching ? movementData.InitCamHeight : crouchCamHeight;

            movementData.IsCrouching = !wasCrouching;
            movementData.CurrentStateHeight = wasCrouching
                ? movementData.InitCamHeight
                : crouchCamHeight;

            var speed = 1f / movementConfig.CrouchTransitionDuration;
            var percent = 0f;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var smoothPercent = movementConfig.CrouchTransitionCurve.Evaluate(percent);
                controller.height = Mathf.Lerp(initHeight, targetHeight, smoothPercent);
                controller.center = Vector3.Lerp(initCenter, targetCenter, smoothPercent);
                camPos.y = Mathf.Lerp(camCurrentHeight, targetCamHeight, smoothPercent);
                yawTransform.localPosition = camPos;

                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }

            movementData.IsDuringCrouchAnimation = false;
        }
    }
}
