using System.Collections;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class LandingHandler
    {
        readonly MonoBehaviour monoBehaviour;
        readonly Transform yawTransform;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerMovementConfig movementConfig;
        IEnumerator landRoutine;

        public LandingHandler(
            MonoBehaviour monoBehaviour,
            Transform yawTransform,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.monoBehaviour = monoBehaviour;
            this.yawTransform = yawTransform;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
        }

        public void HandleLanding(float deltaTime)
        {
            if (collisionData.PreviouslyGrounded || !collisionData.OnGrounded)
                return;

            if (landRoutine != null)
                monoBehaviour.StopCoroutine(landRoutine);

            landRoutine = LandingRoutine(deltaTime);
            monoBehaviour.StartCoroutine(landRoutine);

            //Logging.Log($"HandleLanding: {true}");
        }

        IEnumerator LandingRoutine(float deltaTime)
        {
            var percent = 0f;
            var speed = 1f / movementConfig.LandDuration;

            var localPos = yawTransform.localPosition;
            var initLandHeight = localPos.y;

            var landAmount =
                movementData.InAirTimer > movementConfig.LandTimer
                    ? movementConfig.HighLandAmount
                    : movementConfig.LowLandAmount;

            while (percent < 1f)
            {
                percent += deltaTime * speed;
                var desiredY = movementConfig.LandCurve.Evaluate(percent) * landAmount;

                localPos.y = initLandHeight + desiredY;
                yawTransform.localPosition = localPos;

                yield return null;
            }
        }
    }
}
