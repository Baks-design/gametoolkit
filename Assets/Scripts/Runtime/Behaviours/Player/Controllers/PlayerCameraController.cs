using Alchemy.Inspector;
using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using Unity.Cinemachine;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerCameraController : MonoBehaviour, ILateUpdatable
    {
        [SerializeField, Required]
        Transform pitchTransform;

        [SerializeField, Required]
        Transform yawTransform;

        [SerializeField, Required]
        CinemachineCamera cam;

        [SerializeField]
        PlayerCameraConfig cameraConfig;

        [SerializeField, InlineEditor]
        PerlinNoiseData perlinNoiseConfig;

        [SerializeField, ReadOnly]
        PlayerCameraData cameraData;

        CameraZoom cameraZoom;
        CameraSwaying cameraSwaying;
        CameraRotation cameraRotation;
        CameraBreathing cameraBreathing;
        ILateUpdateServices lateUpdateServices;

        void Awake()
        {
            cameraRotation = new CameraRotation(yawTransform, pitchTransform, cameraConfig);
            cameraBreathing = new CameraBreathing(
                cam.transform,
                cameraConfig,
                perlinNoiseConfig,
                new PlayerMovementData(),
                cameraData
            );
            cameraSwaying = new CameraSwaying(cam.transform, cameraConfig);
            cameraZoom = new CameraZoom(this, cam, cameraConfig, cameraData);
        }

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out lateUpdateServices))
                lateUpdateServices.Register(this);
        }

        public void ProcessLateUpdate(float deltaTime)
        {
            cameraRotation.RotationHandler(deltaTime);
            cameraBreathing.UpdateBreathing(deltaTime);
            cameraZoom.HandleZoom(deltaTime);
        }

        public void HandleSway(Vector3 inputVector, float rawXInput, float deltaTime) =>
            cameraSwaying.SwayPlayer(inputVector, rawXInput, deltaTime);

        public void ChangeRunFOV(bool returning, float deltaTime) =>
            cameraZoom.ChangeRunFOV(returning, deltaTime);

        void OnDisable() => lateUpdateServices.Unregister(this);
    }
}
