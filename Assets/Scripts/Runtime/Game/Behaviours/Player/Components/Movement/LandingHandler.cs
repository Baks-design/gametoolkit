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
        CancellationTokenSource landingCancellationTokenSource;

        public LandingHandler(
            Transform yawTransform,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.yawTransform = yawTransform;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
        }

        public void HandleLanding(float deltaTime)
        {
            if (collisionData.PreviouslyGrounded)
                return;

            // Cancel previous landing if still active
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
            catch (OperationCanceledException)
            {
                // Expected when landing is cancelled
            }
        }

        async UniTask LandingAsync(float deltaTime, CancellationToken cancellationToken = default)
        {
            var percent = 0f;
            var speed = 1f / movementConfig.LandDuration;

            var localPos = yawTransform.localPosition;
            var initLandHeight = localPos.y;

            var landAmount =
                movementData.InAirTimer > movementConfig.LandTimer
                    ? movementConfig.HighLandAmount
                    : movementConfig.LowLandAmount;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var desiredY = movementConfig.LandCurve.Evaluate(percent) * landAmount;

                localPos.y = initLandHeight + desiredY;
                yawTransform.localPosition = localPos;

                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }

            //Logging.Log($"HandleLanding: {true}");
        }
    }
}
