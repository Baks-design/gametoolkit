using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CameraBreathing
    {
        readonly PlayerCameraConfig cameraConfig;
        readonly PerlinNoiseData perlinNoiseConfig;
        readonly PerlinNoiseScroller perlinNoiseScroller;
        readonly Transform cameraTransform;
        readonly PlayerMovementData movementData;
        readonly PlayerCameraData cameraData;

        public CameraBreathing(
            Transform cameraTransform,
            PlayerCameraConfig cameraConfig,
            PerlinNoiseData perlinNoiseConfig,
            PlayerMovementData movementData,
            PlayerCameraData cameraData
        )
        {
            this.cameraTransform = cameraTransform;
            this.movementData = movementData;
            this.cameraTransform = cameraTransform;
            this.cameraData = cameraData;
            this.cameraConfig = cameraConfig;
            this.perlinNoiseConfig = perlinNoiseConfig;

            perlinNoiseScroller = new PerlinNoiseScroller(perlinNoiseConfig, cameraData);
        }

        public void UpdateBreathing(float deltaTime)
        {
            if (movementData.IsMoving)
                return;

            perlinNoiseScroller.UpdateNoise(deltaTime);

            var noise = cameraData.Noise;
            var useX = cameraConfig.X;
            var useY = cameraConfig.Y;
            var useZ = cameraConfig.Z;
            switch (perlinNoiseConfig.TransformTarget)
            {
                case TransformTarget.Position:
                    UpdatePosition(noise, useX, useY, useZ);
                    break;
                case TransformTarget.Rotation:
                    UpdateRotation(noise, useX, useY, useZ);
                    break;
                case TransformTarget.Both:
                    UpdateBoth(noise, useX, useY, useZ);
                    break;
            }

            //Logging.Log($"cameraTransform.localEulerAngles: {cameraTransform.localEulerAngles}");
        }

        void UpdatePosition(Vector3 noise, bool useX, bool useY, bool useZ)
        {
            var currentPos = cameraTransform.localPosition;
            var newPos = new Vector3(
                useX ? noise.x : currentPos.x,
                useY ? noise.y : currentPos.y,
                useZ ? noise.z : currentPos.z
            );
            cameraTransform.localPosition = newPos;
        }

        void UpdateRotation(Vector3 noise, bool useX, bool useY, bool useZ)
        {
            var currentRot = cameraTransform.localEulerAngles;
            var newRot = new Vector3(
                useX ? noise.x : currentRot.x,
                useY ? noise.y : currentRot.y,
                useZ ? noise.z : currentRot.z
            );
            cameraTransform.localEulerAngles = newRot;
        }

        void UpdateBoth(Vector3 noise, bool useX, bool useY, bool useZ)
        {
            var currentPos = cameraTransform.localPosition;
            var currentRot = cameraTransform.localEulerAngles;
            var newPos = new Vector3(
                useX ? noise.x : currentPos.x,
                useY ? noise.y : currentPos.y,
                useZ ? noise.z : currentPos.z
            );
            var newRot = new Vector3(
                useX ? noise.x : currentRot.x,
                useY ? noise.y : currentRot.y,
                useZ ? noise.z : currentRot.z
            );
            cameraTransform.localPosition = newPos;
            cameraTransform.localEulerAngles = newRot;
        }
    }
}
