using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CameraRotation
    {
        readonly PlayerCameraConfig cameraConfig;
        readonly Transform yawTransform;
        readonly Transform pitchTransform;
        PlayerInputData inputData;
        float yaw;
        float pitch;
        float desiredYaw;
        float desiredPitch;

        public CameraRotation(
            Transform yawTransform,
            Transform pitchTransform,
            PlayerCameraConfig cameraConfig
        )
        {
            this.yawTransform = yawTransform;
            this.pitchTransform = pitchTransform;
            this.cameraConfig = cameraConfig;
        }

        public void RotationHandler(float deltaTime)
        {
            CalculateRotation(deltaTime);
            SmoothRotation(deltaTime);
            ApplyRotation();
        }

        void CalculateRotation(float deltaTime)
        {
            desiredYaw += inputData.LookInput.x * cameraConfig.Sensitivity.x * deltaTime;
            desiredPitch -= inputData.LookInput.y * cameraConfig.Sensitivity.y * deltaTime;
            desiredPitch = Mathf.Clamp(
                desiredPitch,
                cameraConfig.LookAngleMinMax.x,
                cameraConfig.LookAngleMinMax.y
            );
        }

        void SmoothRotation(float deltaTime)
        {
            yaw = Mathf.Lerp(yaw, desiredYaw, cameraConfig.SmoothAmount.x * deltaTime);
            pitch = Mathf.Lerp(pitch, desiredPitch, cameraConfig.SmoothAmount.y * deltaTime);
        }

        void ApplyRotation()
        {
            yawTransform.eulerAngles = new Vector3(0f, yaw, 0f);
            pitchTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }
}
