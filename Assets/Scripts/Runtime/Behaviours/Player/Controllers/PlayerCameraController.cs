using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Helpers;
using Unity.Cinemachine;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerCameraController : CustomMonoBehaviour
    {
        [SerializeField]
        Transform pitchTransform;

        [SerializeField]
        Transform yawTransform;

        [SerializeField]
        Transform camTransform;

        [SerializeField]
        CinemachineCamera cam;

        [SerializeField]
        PlayerCameraConfig cameraConfig;

        CameraZoom cameraZoom;
        CameraSwaying cameraSwaying;
        CameraBreathing cameraBreathing;
        CameraRotation cameraRotation;

        protected override void Awake()
        {
            base.Awake();
            GameSystem.SetCursor(true);
            InitalizeComponents();
        }

        void InitalizeComponents()
        {
            cameraRotation = new CameraRotation(yawTransform, pitchTransform, cameraConfig);
            cameraBreathing = new CameraBreathing(camTransform, cameraConfig);
            cameraSwaying = new CameraSwaying(camTransform, cameraConfig);
            cameraZoom = new CameraZoom(this, cam, cameraConfig);
        }

        public override void ProcessLateUpdate(float deltaTime)
        {
            base.ProcessLateUpdate(deltaTime);
            cameraRotation.RotationHandler(deltaTime);
            cameraBreathing.UpdateBreathing(deltaTime);
            cameraZoom.ZoomHandler(deltaTime);
        }

        public void HandleSway(Vector3 inputVector, float rawXInput, float deltaTime) =>
            cameraSwaying.SwayPlayer(inputVector, rawXInput, deltaTime);

        public void ChangeRunFOV(bool returning, float deltaTime) =>
            cameraZoom.ChangeRunFOV(returning, deltaTime);
    }
}
