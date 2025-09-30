using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Helpers;
using Unity.Cinemachine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CameraZoom
    {
        readonly CinemachineCamera cam;
        readonly PlayerCameraConfig cameraConfig;
        readonly PlayerCameraData cameraData;
        readonly float initFOV;
        bool running;
        CancellationTokenSource fovCancellationTokenSource;
        CancellationTokenSource runFovCancellationTokenSource;

        public CameraZoom(
            CinemachineCamera cam,
            PlayerCameraConfig cameraConfig,
            PlayerCameraData cameraData
        )
        {
            this.cam = cam;
            this.cameraConfig = cameraConfig;
            this.cameraData = cameraData;

            initFOV = cam.Lens.FieldOfView;
        }

        public void HandleAimFOV(float deltaTime)
        {
            if (InputManager.AimPressed || InputManager.AimReleased)
                _ = ChangeFOV(deltaTime);

            //Logging.Log($"HandleAimFOV: true");
        }

        async UniTaskVoid ChangeFOV(float deltaTime)
        {
            if (running)
            {
                cameraData.IsZooming = !cameraData.IsZooming;
                return;
            }

            // Cancel previous FOV tasks
            fovCancellationTokenSource?.Cancel();
            fovCancellationTokenSource = new CancellationTokenSource();

            // Also cancel run FOV if it's running
            runFovCancellationTokenSource?.Cancel();

            try
            {
                await ChangeFOVAsync(deltaTime, fovCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled, this is expected
            }
        }

        public void HandleRunFOV(bool returning, float deltaTime)
        {
            _ = ChangeRunFOV(returning, deltaTime);

            //Logging.Log($"HandleRunFOV: true");
        }

        async UniTaskVoid ChangeRunFOV(bool returning, float deltaTime)
        {
            // Cancel previous run FOV tasks
            runFovCancellationTokenSource?.Cancel();
            runFovCancellationTokenSource = new CancellationTokenSource();

            // Also cancel regular FOV if it's running
            fovCancellationTokenSource?.Cancel();

            try
            {
                await ChangeRunFOVAsync(returning, deltaTime, runFovCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled, this is expected
            }
        }

        async UniTask ChangeFOVAsync(float deltaTime, CancellationToken cancellationToken = default)
        {
            var percent = 0f;
            var speed = 1f / cameraConfig.ZoomTransitionDuration;

            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = cameraData.IsZooming ? initFOV : cameraConfig.ZoomFOV;

            cameraData.IsZooming = !cameraData.IsZooming;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var smoothPercent = cameraConfig.ZoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathfs.Eerp(currentFOV, targetFOV, smoothPercent);
                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        async UniTask ChangeRunFOVAsync(
            bool returning,
            float deltaTime,
            CancellationToken cancellationToken = default
        )
        {
            var percent = 0f;
            var duration = returning
                ? cameraConfig.RunReturnTransitionDuration
                : cameraConfig.RunTransitionDuration;
            var speed = 1f / duration;

            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : cameraConfig.RunFOV;

            running = !returning;

            while (percent < 1f)
            {
                cancellationToken.ThrowIfCancellationRequested();

                percent += deltaTime * speed;
                var smoothPercent = cameraConfig.RunCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathfs.Eerp(currentFOV, targetFOV, smoothPercent);
                await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
            }
        }
    }
}
