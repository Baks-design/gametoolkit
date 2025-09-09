using System.Collections;
using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using Unity.Cinemachine;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CameraZoom
    {
        readonly MonoBehaviour mono;
        readonly CinemachineCamera cam;
        readonly PlayerCameraConfig cameraConfig;
        readonly PlayerCameraData cameraData;
        readonly float initFOV;
        bool running;
        IEnumerator changeFOVRoutine;
        IEnumerator changeRunFOVRoutine;

        public CameraZoom(
            MonoBehaviour mono,
            CinemachineCamera cam,
            PlayerCameraConfig cameraConfig,
            PlayerCameraData cameraData
        )
        {
            this.mono = mono;
            this.cam = cam;
            this.cameraConfig = cameraConfig;
            this.cameraData = cameraData;

            initFOV = cam.Lens.FieldOfView;
        }

        public void HandleZoom(float deltaTime)
        {
            if (InputManager.AimPressed || InputManager.AimReleased)
                ChangeFOV(deltaTime);

            //Logging.Log($"HandleZoom: {true}");
        }

        public void ChangeFOV(float deltaTime)
        {
            if (running)
            {
                cameraData.IsZooming = !cameraData.IsZooming;
                return;
            }

            if (changeRunFOVRoutine != null)
                mono.StopCoroutine(changeRunFOVRoutine);
            if (changeFOVRoutine != null)
                mono.StopCoroutine(changeFOVRoutine);

            changeFOVRoutine = ChangeFOVRoutine(deltaTime);
            mono.StartCoroutine(changeFOVRoutine);

            //Logging.Log($"ChangeFOV: {true}");
        }

        IEnumerator ChangeFOVRoutine(float deltaTime)
        {
            var percent = 0f;
            var speed = 1f / cameraConfig.ZoomTransitionDuration;

            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = cameraData.IsZooming ? initFOV : cameraConfig.ZoomFOV;

            cameraData.IsZooming = !cameraData.IsZooming;

            while (percent < 1f)
            {
                percent += deltaTime * speed;
                var smoothPercent = cameraConfig.ZoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathfs.Eerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool returning, float deltaTime)
        {
            if (changeFOVRoutine != null)
                mono.StopCoroutine(changeFOVRoutine);
            if (changeRunFOVRoutine != null)
                mono.StopCoroutine(changeRunFOVRoutine);

            changeRunFOVRoutine = ChangeRunFOVRoutine(returning, deltaTime);
            mono.StartCoroutine(changeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning, float deltaTime)
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
                percent += deltaTime * speed;
                var smoothPercent = cameraConfig.RunCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathfs.Eerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }
    }
}
