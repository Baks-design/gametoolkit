using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [CreateAssetMenu(menuName = "Data/HeadBobData")]
    public class HeadBobData : ScriptableObject
    {
        [Header("Curves")]
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;

        [Header("Amplitude")]
        public float xAmplitude;
        public float yAmplitude;

        [Header("Frequency")]
        public float xFrequency;
        public float yFrequency;

        [Header("Run Multipliers")]
        public float runAmplitudeMultiplier;
        public float runFrequencyMultiplier;

        [Header("Crouch Multipliers")]
        public float crouchAmplitudeMultiplier;
        public float crouchFrequencyMultiplier;

        public float MoveBackwardsFrequencyMultiplier { get; set; }
        public float MoveSideFrequencyMultiplier { get; set; }
    }

    public class HeadBob
    {
        readonly HeadBobData data;
        Vector3 finalOffset;
        float xScroll;
        float yScroll;
        bool resetted;
        float currentStateHeight = 0f;

        public Vector3 FinalOffset => finalOffset;
        public bool Resetted => resetted;
        public float CurrentStateHeight
        {
            get => currentStateHeight;
            set => currentStateHeight = value;
        }

        public HeadBob(HeadBobData data, float moveBackwardsMultiplier, float moveSideMultiplier)
        {
            this.data = data;
            data.MoveBackwardsFrequencyMultiplier = moveBackwardsMultiplier;
            data.MoveSideFrequencyMultiplier = moveSideMultiplier;
            xScroll = yScroll = 0f;
            resetted = false;
            finalOffset = Vector3.zero;
        }

        public void ScrollHeadBob(bool running, bool crouching, Vector2 input)
        {
            resetted = false;

            float amplitudeMultiplier;
            float frequencyMultiplier;
            float additionalMultiplier; // when moving backwards or to sides

            amplitudeMultiplier = running ? data.runAmplitudeMultiplier : 1f;
            amplitudeMultiplier = crouching ? data.crouchAmplitudeMultiplier : amplitudeMultiplier;

            frequencyMultiplier = running ? data.runFrequencyMultiplier : 1f;
            frequencyMultiplier = crouching ? data.crouchFrequencyMultiplier : frequencyMultiplier;

            additionalMultiplier = input.y == -1f ? data.MoveBackwardsFrequencyMultiplier : 1f;
            additionalMultiplier =
                input.x != 0f && input.y == 0f
                    ? data.MoveSideFrequencyMultiplier
                    : additionalMultiplier;

            // you can also multiply this by additionalMultiplier but it looks unnatural a bit;
            xScroll += Time.deltaTime * data.xFrequency * frequencyMultiplier;

            float xValue;
            float yValue;

            xValue = data.xCurve.Evaluate(xScroll);
            yValue = data.yCurve.Evaluate(yScroll);

            finalOffset.x = xValue * data.xAmplitude * amplitudeMultiplier * additionalMultiplier;
            finalOffset.y = yValue * data.yAmplitude * amplitudeMultiplier * additionalMultiplier;
        }

        public void ResetHeadBob()
        {
            resetted = true;
            xScroll = yScroll = 0f;
            finalOffset = Vector3.zero;
        }
    }

    public class PlayerMovement
    {
        readonly CharacterController controller;
        readonly Transform targetRotation;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCameraController cameraController;
        readonly HeadBobData headBobConfig;
        readonly HeadBob headBob;
        PlayerCollisionData collisionData;
        PlayerMovementData movementData;
        PlayerInputData inputData;
        Vector3 smoothFinalMoveDir;
        Vector2 smoothInputVector;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;
        readonly float initCamHeight;
        float currentSpeed;
        bool duringRunAnimation;
        readonly bool duringCrouchAnimation;
        readonly float walkRunSpeedDifference;

        public PlayerMovement(
            CharacterController controller,
            Transform targetRotation,
            PlayerMovementConfig movementConfig,
            HeadBobData headBobConfig,
            PlayerCameraController cameraController
        )
        {
            this.controller = controller;
            this.targetRotation = targetRotation;
            this.movementConfig = movementConfig;
            this.headBobConfig = headBobConfig;
            this.cameraController = cameraController;

            initCamHeight = targetRotation.localPosition.y;
            headBob = new HeadBob(
                headBobConfig,
                movementConfig.MoveBackwardsSpeedPercent,
                movementConfig.MoveSideSpeedPercent
            )
            {
                CurrentStateHeight = initCamHeight
            };
            movementData.InAirTimer = 0f;
            walkRunSpeedDifference = movementConfig.RunSpeed - movementConfig.WalkSpeed;
        }

        public void RotateTowardsCamera(float deltaTime) =>
            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation,
                targetRotation.rotation,
                deltaTime * movementConfig.SmoothRotateSpeed
            );

        #region Apply Smoothing
        public void SmoothInput(float deltaTime) =>
            smoothInputVector = Vector2.Lerp(
                smoothInputVector,
                inputData.MoveInput.normalized,
                deltaTime * movementConfig.SmoothInputSpeed
            );

        public void SmoothSpeed(float deltaTime)
        {
            smoothCurrentSpeed = Mathf.Lerp(
                smoothCurrentSpeed,
                currentSpeed,
                deltaTime * movementConfig.SmoothVelocitySpeed
            );

            if (movementData.IsRunning && CanRun())
            {
                var walkRunPercent = Mathf.InverseLerp(
                    movementConfig.WalkSpeed,
                    movementConfig.RunSpeed,
                    smoothCurrentSpeed
                );
                finalSmoothCurrentSpeed =
                    movementConfig.RunTransitionCurve.Evaluate(walkRunPercent)
                        * walkRunSpeedDifference
                    + movementConfig.WalkSpeed;
            }
            else
                finalSmoothCurrentSpeed = smoothCurrentSpeed;
        }

        public void SmoothDirection(float deltaTime) =>
            smoothFinalMoveDir = Vector3.Lerp(
                smoothFinalMoveDir,
                movementData.FinalMoveDirection,
                deltaTime * movementConfig.SmoothFinalDirectionSpeed
            );
        #endregion

        #region Calculate Movement
        public void CalculateMovementDirection()
        {
            var vDir = controller.transform.forward * smoothInputVector.y;
            var hDir = controller.transform.right * smoothInputVector.x;
            var desiredDir = vDir + hDir;
            var flattenDir = FlattenVectorOnSlopes(desiredDir);
            movementData.FinalMoveDirection = flattenDir;
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
        {
            if (!collisionData.OnGrounded)
                return vectorToFlat;
            vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat, collisionData.GroundedNormal);
            return vectorToFlat;
        }

        public void CalculateSpeed()
        {
            currentSpeed =
                movementData.IsRunning && CanRun()
                    ? movementConfig.RunSpeed
                    : movementConfig.WalkSpeed;
            currentSpeed = !inputData.HasMoveInput ? 0f : currentSpeed;
            currentSpeed =
                inputData.MoveInput.y == -1f
                    ? currentSpeed * movementConfig.MoveBackwardsSpeedPercent
                    : currentSpeed;
            currentSpeed =
                inputData.MoveInput.x != 0f && inputData.MoveInput.y == 0f
                    ? currentSpeed * movementConfig.MoveSideSpeedPercent
                    : currentSpeed;
        }

        bool CanRun()
        {
            var normalizedDir = Vector3.zero;
            if (smoothFinalMoveDir != Vector3.zero)
                normalizedDir = smoothFinalMoveDir.normalized;
            var dot = Vector3.Dot(controller.transform.forward, normalizedDir);
            return dot >= movementConfig.CanRunThreshold;
        }

        public void CalculateFinalMovement()
        {
            var finalVector = finalSmoothCurrentSpeed * smoothFinalMoveDir;
            movementData.FinalMoveVelocity.x = finalVector.x;
            movementData.FinalMoveVelocity.z = finalVector.z;
            if (controller.isGrounded)
                movementData.FinalMoveVelocity.y += finalVector.y;
        }
        #endregion

        public void HandleJump(float deltaTime)
        {
            if (!inputData.JumpPressed || !controller.isGrounded || inputData.IsCrouching)
                return;

            movementData.FinalMoveVelocity.y = Mathf.Sqrt(
                movementConfig.JumpSpeed * deltaTime * 2f
            );
            collisionData.PreviouslyGrounded = true;
            collisionData.OnGrounded = false;
        }

        #region Apply Movement
        public void ApplyGravityOnGrounded()
        {
            if (!controller.isGrounded)
                return;

            movementData.InAirTimer = 0f;
            movementData.FinalMoveVelocity.y = -movementConfig.StickToGroundForce;
        }

        public void ApplyGravityOnAirborne(float deltaTime)
        {
            if (controller.isGrounded)
                return;

            movementData.InAirTimer += deltaTime;
            movementData.FinalMoveVelocity +=
                deltaTime * movementConfig.GravityMultiplier * Physics.gravity;
        }

        public void ApplyMove(float deltaTime)
        {
            controller.Move(movementData.FinalMoveVelocity * deltaTime);

            movementData.IsMoving = controller.velocity.sqrMagnitude > 0.1f;
            movementData.IsWalking = controller.velocity.x > 0.1f || controller.velocity.y > 0.1f;
        }
        #endregion

        #region Locomotion Apply Methods
        public void HandleHeadBob(float deltaTime)
        {
            if (inputData.HasMoveInput && collisionData.OnGrounded && !collisionData.HasObstructed)
            {
                // we want to make our head bob only if we are moving and not during crouch routine
                if (!duringCrouchAnimation)
                {
                    headBob.ScrollHeadBob(
                        inputData.IsRunning && CanRun(),
                        inputData.IsCrouching,
                        inputData.MoveInput
                    );
                    targetRotation.localPosition = Vector3.Lerp(
                        targetRotation.localPosition,
                        (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                        deltaTime * movementConfig.SmoothHeadBobSpeed
                    );
                }
            }
            else // if we are not moving or we are not grounded
            {
                if (!headBob.Resetted)
                    headBob.ResetHeadBob();

                // we want to reset our head bob only if we are standing still and not during crouch routine
                if (!duringCrouchAnimation)
                    targetRotation.localPosition = Vector3.Lerp(
                        targetRotation.localPosition,
                        new Vector3(0f, headBob.CurrentStateHeight, 0f),
                        deltaTime * movementConfig.SmoothHeadBobSpeed
                    );
            }
        }

        public void HandleCameraSway(float deltaTime) =>
            cameraController.HandleSway(smoothInputVector, inputData.MoveInput.x, deltaTime);

        public void HandleRunFOV(float deltaTime)
        {
            if (inputData.HasMoveInput && collisionData.OnGrounded && !collisionData.HasObstructed)
            {
                if (inputData.RunPressed && CanRun())
                {
                    duringRunAnimation = true;
                    cameraController.ChangeRunFOV(false, deltaTime);
                }

                if (inputData.IsRunning && CanRun() && !duringRunAnimation)
                {
                    duringRunAnimation = true;
                    cameraController.ChangeRunFOV(false, deltaTime);
                }
            }

            if (inputData.RunReleased || !inputData.HasMoveInput || collisionData.HasObstructed)
            {
                if (duringRunAnimation)
                {
                    duringRunAnimation = false;
                    cameraController.ChangeRunFOV(true, deltaTime);
                }
            }
        }
        #endregion
    }
}
