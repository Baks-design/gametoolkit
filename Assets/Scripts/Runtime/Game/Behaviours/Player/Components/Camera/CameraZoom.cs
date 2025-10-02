using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Helpers;
using Unity.Cinemachine;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CameraZoom
    {
        readonly IMovementInput movementInput;
        readonly CinemachineCamera cam;
        readonly PlayerCameraConfig cameraConfig;
        readonly PlayerCameraData cameraData;
        readonly float initFOV;
        readonly CancellationTokenSource fovCancellationTokenSource;
        readonly CancellationTokenSource runFovCancellationTokenSource;
        bool running;

        public CameraZoom(
            IMovementInput movementInput,
            CinemachineCamera cam,
            PlayerCameraConfig cameraConfig,
            PlayerCameraData cameraData
        )
        {
            this.movementInput = movementInput;
            this.cam = cam;
            this.cameraConfig = cameraConfig;
            this.cameraData = cameraData;

            initFOV = cam.Lens.FieldOfView;
        }

        public void HandleAimFOV(float deltaTime)
        {
            if (movementInput.AimPressed() || movementInput.AimReleased())
                _ = ChangeFOV(deltaTime);
        }

        public void HandleRunFOV(bool returning, float deltaTime) =>
            _ = ChangeRunFOV(returning, deltaTime);

        async UniTaskVoid ChangeFOV(float deltaTime)
        {
            if (running)
            {
                cameraData.IsZooming = !cameraData.IsZooming;
                return;
            }

            await ExecuteFOVChangeAsync(
                fovCancellationTokenSource,
                () => runFovCancellationTokenSource?.Cancel(),
                ct => ChangeFOVAsync(deltaTime, ct)
            );
        }

        async UniTaskVoid ChangeRunFOV(bool returning, float deltaTime) =>
            await ExecuteFOVChangeAsync(
                runFovCancellationTokenSource,
                () => fovCancellationTokenSource?.Cancel(),
                ct => ChangeRunFOVAsync(returning, deltaTime, ct)
            );

        async UniTask ExecuteFOVChangeAsync(
            CancellationTokenSource source,
            Action cancelOther,
            Func<CancellationToken, UniTask> fovTask
        )
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            cancelOther();

            try
            {
                await fovTask(source.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
        }

        async UniTask ChangeFOVAsync(float deltaTime, CancellationToken cancellationToken)
        {
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = cameraData.IsZooming ? initFOV : cameraConfig.ZoomFOV;
            cameraData.IsZooming = !cameraData.IsZooming;

            await AnimateFOV(
                currentFOV,
                targetFOV,
                cameraConfig.ZoomTransitionDuration,
                cameraConfig.ZoomCurve,
                deltaTime,
                cancellationToken
            );
        }

        async UniTask ChangeRunFOVAsync(
            bool returning,
            float deltaTime,
            CancellationToken cancellationToken
        )
        {
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : cameraConfig.RunFOV;
            var duration = returning
                ? cameraConfig.RunReturnTransitionDuration
                : cameraConfig.RunTransitionDuration;

            running = !returning;

            await AnimateFOV(
                currentFOV,
                targetFOV,
                duration,
                cameraConfig.RunCurve,
                deltaTime,
                cancellationToken
            );
        }

        async UniTask AnimateFOV(
            float currentFOV,
            float targetFOV,
            float duration,
            AnimationCurve curve,
            float deltaTime,
            CancellationToken cancellationToken
        )
        {
            if (duration <= 0f)
            {
                cam.Lens.FieldOfView = targetFOV;
                return;
            }

            var percent = 0f;
            var speed = 1f / duration;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var smoothPercent = curve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathfs.Eerp(currentFOV, targetFOV, smoothPercent);

                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }

            // Ensure final value is set exactly
            cam.Lens.FieldOfView = targetFOV;
        }
    }
}
