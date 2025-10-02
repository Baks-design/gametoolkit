using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class LandingHandler
    {
        readonly Transform yawTransform;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerMovementConfig movementConfig;
        readonly IPlayerSound sound;
        readonly IPlayerAnimation animation;
        CancellationTokenSource landingCancellationTokenSource;

        public LandingHandler(
            Transform yawTransform,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig,
            IPlayerSound sound,
            IPlayerAnimation animation
        )
        {
            this.yawTransform = yawTransform;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
            this.sound = sound;
            this.animation = animation;
        }

        public void HandleLanding(float deltaTime)
        {
            if (collisionData.PreviouslyGrounded)
                return;

            landingCancellationTokenSource?.Cancel();
            landingCancellationTokenSource = new CancellationTokenSource();

            _ = StartLanding(deltaTime, landingCancellationTokenSource.Token);
        }

        async UniTaskVoid StartLanding(
            float deltaTime,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                await LandingAsync(deltaTime, cancellationToken);
            }
            catch (OperationCanceledException) { }
        }

        async UniTask LandingAsync(float deltaTime, CancellationToken cancellationToken = default)
        {
            var percent = 0f;
            var speed = 1f / movementConfig.LandDuration;
            var localPos = yawTransform.localPosition;
            var initialHeight = localPos.y;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var curveValue = movementConfig.LandCurve.Evaluate(percent);
                var desiredOffset = curveValue * CalculateLandAmount();
                localPos.y = initialHeight + desiredOffset;
                yawTransform.localPosition = localPos;

                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        float CalculateLandAmount() =>
            movementData.InAirTimer > movementConfig.LandTimer
                ? movementConfig.HighLandAmount
                : movementConfig.LowLandAmount;
    }
}
