using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CameraZoom
    {
        readonly MonoBehaviour mono;
        readonly CinemachineCamera cam;
        readonly PlayerCameraConfig cameraConfig;
        readonly float initFOV;
        bool running;
        IEnumerator changeFOVRoutine;
        IEnumerator changeRunFOVRoutine;
        PlayerInputData inputData;
        PlayerCameraData cameraData;

        public CameraZoom(
            MonoBehaviour mono,
            CinemachineCamera cam,
            PlayerCameraConfig cameraConfig
        )
        {
            this.mono = mono;
            this.cam = cam;
            this.cameraConfig = cameraConfig;
            initFOV = cam.Lens.FieldOfView;
        }

        public void ZoomHandler(float deltaTime)
        {
            if (inputData.ZoomPressed || inputData.ZoomReleased)
                ChangeFOV(deltaTime);
        }

        public void ChangeFOV(float deltaTime)
        {
            if (running)
            {
                inputData.IsZooming = !inputData.IsZooming;
                cameraData.IsZooming = inputData.IsZooming;
                return;
            }

            if (changeRunFOVRoutine != null)
                mono.StopCoroutine(changeRunFOVRoutine);
            if (changeFOVRoutine != null)
                mono.StopCoroutine(changeFOVRoutine);

            changeFOVRoutine = ChangeFOVRoutine(deltaTime);
            mono.StartCoroutine(changeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine(float deltaTime)
        {
            var percent = 0f;
            var speed = 1f / cameraConfig.ZoomTransitionDuration;

            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = inputData.IsZooming ? initFOV : cameraConfig.ZoomFOV;

            inputData.IsZooming = !inputData.IsZooming;
            cameraData.IsZooming = inputData.IsZooming;

            while (percent < 1f)
            {
                percent += deltaTime * speed;
                var smoothPercent = cameraConfig.ZoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
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
                cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }
    }
}
