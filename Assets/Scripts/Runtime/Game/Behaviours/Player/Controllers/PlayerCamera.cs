using System;
using Alchemy.Inspector;
using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using Unity.Cinemachine;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public class PlayerCamera : MonoBehaviour, IPlayerCamera
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
        PerlinNoiseConfig perlinNoiseConfig;

        [SerializeField, ReadOnly]
        PlayerCameraData cameraData;

        CameraZoom cameraZoom;
        CameraSwaying cameraSwaying;
        CameraRotation cameraRotation;
        CameraBreathing cameraBreathing;
        IMovementInput movementInput;

        void OnEnable() => ServiceLocator.Global.Get(out movementInput);

        void Start()
        {
            cameraRotation = new CameraRotation(
                movementInput,
                yawTransform,
                pitchTransform,
                cameraConfig
            );
            cameraBreathing = new CameraBreathing(
                cam.transform,
                cameraConfig,
                perlinNoiseConfig,
                new PlayerMovementData(),
                cameraData
            );
            cameraSwaying = new CameraSwaying(cam.transform, cameraConfig);
            cameraZoom = new CameraZoom(movementInput, cam, cameraConfig, cameraData);
        }

        public void BreathingHandler(float deltaTime) => cameraBreathing.UpdateBreathing(deltaTime);

        public void RotationHandler(float deltaTime) => cameraRotation.RotationHandler(deltaTime);

        public void SwayHandler(Vector3 inputVector, float rawXInput, float deltaTime) =>
            cameraSwaying.SwayPlayer(inputVector, rawXInput, deltaTime);

        public void AimHandler(float deltaTime) => cameraZoom.HandleAimFOV(deltaTime);

        public void RunFOVHandler(bool returning, float deltaTime) =>
            cameraZoom.HandleRunFOV(returning, deltaTime);
    }
}
