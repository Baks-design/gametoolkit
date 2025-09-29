using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CameraSwaying
    {
        readonly PlayerCameraConfig cameraConfig;
        readonly Transform camTransform;
        float scrollSpeed;
        float xAmountThisFrame;
        float xAmountPreviousFrame;
        bool differentDirection;

        public CameraSwaying(Transform camTransform, PlayerCameraConfig cameraConfig)
        {
            this.camTransform = camTransform;
            this.cameraConfig = cameraConfig;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput, float deltaTime)
        {
            var xAmount = inputVector.x;

            xAmountThisFrame = rawXInput;

            if (rawXInput != 0f) // if we have some input
            {
                // if our previous dir is not equal to current one and the previous one was not idle
                if (xAmountThisFrame != xAmountPreviousFrame && xAmountPreviousFrame != 0f)
                    differentDirection = true;

                // then we multiplier our scroll so when changing direction it will sway to the other direction faster
                var speedMultiplier = differentDirection
                    ? cameraConfig.ChangeDirectionMultiplier
                    : 1f;
                scrollSpeed += xAmount * cameraConfig.SwaySpeed * deltaTime * speedMultiplier;
            }
            else // if we are not moving so there is no input
            {
                if (xAmountThisFrame == xAmountPreviousFrame) // check if our previous dir equals current dir
                    differentDirection = false; // if yes we want to reset this bool so basically it can be used correctly once we move again

                scrollSpeed = Mathf.Lerp(scrollSpeed, 0f, deltaTime * cameraConfig.ReturnSpeed);
            }

            scrollSpeed = Mathf.Clamp(scrollSpeed, -1f, 1f);

            float swayFinalAmount;
            if (scrollSpeed < 0f)
                swayFinalAmount =
                    -cameraConfig.SwayCurve.Evaluate(scrollSpeed) * -cameraConfig.SwayAmount;
            else
                swayFinalAmount =
                    cameraConfig.SwayCurve.Evaluate(scrollSpeed) * -cameraConfig.SwayAmount;

            Vector3 swayVector;
            swayVector.z = swayFinalAmount;

            camTransform.localEulerAngles = new Vector3(
                camTransform.localEulerAngles.x,
                camTransform.localEulerAngles.y,
                swayVector.z
            );

            xAmountPreviousFrame = xAmountThisFrame;

            //Logging.Log($"SwayPlayer: {true}");
        }
    }
}
