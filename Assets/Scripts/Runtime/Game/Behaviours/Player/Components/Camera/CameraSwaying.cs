using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CameraSwaying
    {
        readonly PlayerCameraConfig cameraConfig;
        readonly Transform camTransform;
        bool differentDirection;
        float scrollSpeed;
        float xAmountThisFrame;
        float xAmountPreviousFrame;

        public CameraSwaying(Transform camTransform, PlayerCameraConfig cameraConfig)
        {
            this.camTransform = camTransform;
            this.cameraConfig = cameraConfig;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput, float deltaTime)
        {
            xAmountThisFrame = rawXInput;

            if (rawXInput != 0f) //HasInput
            {
                // Check if direction changed (and previous wasn't idle)
                differentDirection = (
                    xAmountThisFrame != xAmountPreviousFrame && xAmountPreviousFrame != 0f
                );

                var speedMultiplier = differentDirection
                    ? cameraConfig.ChangeDirectionMultiplier
                    : 1f;
                scrollSpeed += inputVector.x * cameraConfig.SwaySpeed * deltaTime * speedMultiplier;
            }
            else
            {
                // Reset direction flag if we've been idle for consecutive frames
                if (xAmountThisFrame == xAmountPreviousFrame)
                    differentDirection = false;

                scrollSpeed = Mathf.Lerp(scrollSpeed, 0f, deltaTime * cameraConfig.ReturnSpeed);
            }

            scrollSpeed = Mathf.Clamp(scrollSpeed, -1f, 1f);

            // Calculate sway amount
            var curveValue = cameraConfig.SwayCurve.Evaluate(Mathf.Abs(scrollSpeed));
            var swayFinalAmount = curveValue * -cameraConfig.SwayAmount;
            if (scrollSpeed < 0f)
                swayFinalAmount = -swayFinalAmount;

            // Apply sway to Z rotation only
            var currentEuler = camTransform.localEulerAngles;
            camTransform.localEulerAngles = new Vector3(
                currentEuler.x,
                currentEuler.y,
                swayFinalAmount
            );

            xAmountPreviousFrame = xAmountThisFrame;

            //Logging.Log($"SwayPlayer: {true}");
        }
    }
}
