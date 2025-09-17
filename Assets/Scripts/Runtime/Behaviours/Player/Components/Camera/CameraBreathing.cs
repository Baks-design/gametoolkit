using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CameraBreathing
    {
        readonly PlayerCameraConfig cameraConfig;
        readonly PerlinNoiseData perlinNoiseConfig;
        readonly PerlinNoiseScroller perlinNoiseScroller;
        readonly Transform cameraTransform;
        readonly PlayerMovementData movementData;
        readonly PlayerCameraData cameraData;
        Vector3 finalRot;
        Vector3 finalPos;

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

            var posOffset = Vector3.zero;
            var rotOffset = Vector3.zero;

            switch (perlinNoiseConfig.TransformTarget)
            {
                case TransformTarget.Position:
                {
                    if (cameraConfig.X)
                        posOffset.x += cameraData.Noise.x;

                    if (cameraConfig.Y)
                        posOffset.y += cameraData.Noise.y;

                    if (cameraConfig.Z)
                        posOffset.z += cameraData.Noise.z;

                    finalPos.x = cameraConfig.X ? posOffset.x : cameraTransform.localPosition.x;
                    finalPos.y = cameraConfig.Y ? posOffset.y : cameraTransform.localPosition.y;
                    finalPos.z = cameraConfig.Z ? posOffset.z : cameraTransform.localPosition.z;

                    cameraTransform.localPosition = finalPos;
                    break;
                }
                case TransformTarget.Rotation:
                {
                    if (cameraConfig.X)
                        rotOffset.x += cameraData.Noise.x;

                    if (cameraConfig.Y)
                        rotOffset.y += cameraData.Noise.y;

                    if (cameraConfig.Z)
                        rotOffset.z += cameraData.Noise.z;

                    finalRot.x = cameraConfig.X ? rotOffset.x : cameraTransform.localEulerAngles.x;
                    finalRot.y = cameraConfig.Y ? rotOffset.y : cameraTransform.localEulerAngles.y;
                    finalRot.z = cameraConfig.Z ? rotOffset.z : cameraTransform.localEulerAngles.z;

                    cameraTransform.localEulerAngles = finalRot;

                    break;
                }
                case TransformTarget.Both:
                {
                    if (cameraConfig.X)
                    {
                        posOffset.x += cameraData.Noise.x;
                        rotOffset.x += cameraData.Noise.x;
                    }

                    if (cameraConfig.Y)
                    {
                        posOffset.y += cameraData.Noise.y;
                        rotOffset.y += cameraData.Noise.y;
                    }

                    if (cameraConfig.Z)
                    {
                        posOffset.z += cameraData.Noise.z;
                        rotOffset.z += cameraData.Noise.z;
                    }

                    finalPos.x = cameraConfig.X ? posOffset.x : cameraTransform.localPosition.x;
                    finalPos.y = cameraConfig.Y ? posOffset.y : cameraTransform.localPosition.y;
                    finalPos.z = cameraConfig.Z ? posOffset.z : cameraTransform.localPosition.z;

                    finalRot.x = cameraConfig.X ? rotOffset.x : cameraTransform.localEulerAngles.x;
                    finalRot.y = cameraConfig.Y ? rotOffset.y : cameraTransform.localEulerAngles.y;
                    finalRot.z = cameraConfig.Z ? rotOffset.z : cameraTransform.localEulerAngles.z;

                    cameraTransform.localPosition = finalPos;
                    cameraTransform.localEulerAngles = finalRot;

                    break;
                }
            }

            //Logging.Log($"cameraTransform.localPosition: {cameraTransform.localPosition}");
        }
    }
}
