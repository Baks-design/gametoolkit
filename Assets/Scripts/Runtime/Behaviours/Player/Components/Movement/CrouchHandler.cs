using System.Collections;
using GameToolkit.Runtime.Systems.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CrouchHandler
    {
        readonly MonoBehaviour monoBehaviour;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly float crouchCamHeight;
        readonly float initCamHeight;
        readonly float crouchHeight;
        readonly float crouchStandHeightDifference;
        IEnumerator crouchRoutine;
        Vector3 crouchCenter;

        public CrouchHandler(
            MonoBehaviour monoBehaviour,
            CharacterController controller,
            Transform yawTransform,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData
        )
        {
            this.monoBehaviour = monoBehaviour;
            this.controller = controller;
            this.yawTransform = yawTransform;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.movementData = movementData;

            initCamHeight = yawTransform.localPosition.y;

            crouchHeight = collisionData.InitHeight * movementConfig.CrouchPercent;
            crouchCenter = (crouchHeight / 2f + controller.skinWidth) * Vector3.up;
            crouchStandHeightDifference = collisionData.InitHeight - crouchHeight;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;

            movementData.CurrentStateHeight = initCamHeight;
        }

        public void HandleCrouch(float deltaTime)
        {
            if (!InputManager.CrouchPressed)
                return;

            if (movementData.IsCrouching)
                if (collisionData.HasRoofed)
                    return;

            if (movementData.LandRoutine != null)
                monoBehaviour.StopCoroutine(movementData.LandRoutine);

            if (crouchRoutine != null)
                monoBehaviour.StopCoroutine(crouchRoutine);

            crouchRoutine = CrouchRoutine(deltaTime);
            monoBehaviour.StartCoroutine(crouchRoutine);
        }

        IEnumerator CrouchRoutine(float deltaTime)
        {
            movementData.IsDuringCrouchAnimation = true;

            //Logging.Log($"movementData.IsDuringCrouchAnimation: {movementData.IsDuringCrouchAnimation}");

            var percent = 0f;
            var speed = 1f / movementConfig.CrouchTransitionDuration;

            var currentHeight = controller.height;
            var currentCenter = controller.center;

            var desiredHeight = movementData.IsCrouching ? collisionData.InitHeight : crouchHeight;
            var desiredCenter = movementData.IsCrouching ? collisionData.InitCenter : crouchCenter;

            var camPos = yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var camDesiredHeight = movementData.IsCrouching ? initCamHeight : crouchCamHeight;

            movementData.IsCrouching = !movementData.IsCrouching;
            movementData.CurrentStateHeight = movementData.IsCrouching
                ? crouchCamHeight
                : initCamHeight;

            while (percent < 1f)
            {
                percent += deltaTime * speed;
                var smoothPercent = movementConfig.CrouchTransitionCurve.Evaluate(percent);

                controller.height = Mathf.Lerp(currentHeight, desiredHeight, smoothPercent);
                controller.center = Vector3.Lerp(currentCenter, desiredCenter, smoothPercent);

                camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, smoothPercent);
                yawTransform.localPosition = camPos;

                yield return null;
            }

            movementData.IsDuringCrouchAnimation = false;
        }
    }
}
