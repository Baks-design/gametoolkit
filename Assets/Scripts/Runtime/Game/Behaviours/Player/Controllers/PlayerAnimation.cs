using Alchemy.Inspector;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerAnimation : MonoBehaviour, IPlayerAnimation
    {
        [SerializeField, Required]
        Animator animator;

        [SerializeField, HideInInspector]
        PlayerMovementData movementData;

        [SerializeField, HideInInspector]
        PlayerMovementConfig movementConfig;

        [SerializeField, HideInInspector]
        PlayerCollisionData collisionData;

        float currentSpeed;
        bool wasGrounded;
        bool wasSwimming;
        bool wasClimbing;

        readonly int SpeedId = Animator.StringToHash("_Speed");
        readonly int IsGroundedId = Animator.StringToHash("_IsGrounded");
        readonly int IsJumpingId = Animator.StringToHash("_IsJumping");
        readonly int IsFallingId = Animator.StringToHash("_IsFalling");
        readonly int IsCrouchingId = Animator.StringToHash("_IsCrouching");
        readonly int IsSwimmingId = Animator.StringToHash("_IsSwimming");
        readonly int IsClimbingId = Animator.StringToHash("_IsClimbing");
        readonly int ClimbSpeedId = Animator.StringToHash("_ClimbSpeed");
        readonly int SwimSpeedId = Animator.StringToHash("_SwimSpeed");
        readonly int VerticalVelocityId = Animator.StringToHash("_VerticalVelocity");
        readonly int LandId = Animator.StringToHash("_Land");
        readonly int EnterWaterId = Animator.StringToHash("_EnterWater");
        readonly int ExitWaterId = Animator.StringToHash("_ExitWater");
        readonly int StartClimbingId = Animator.StringToHash("_StartClimbing");
        readonly int StopClimbingId = Animator.StringToHash("_StopClimbing");

        public void UpdateMoving()
        {
            var targetSpeed = movementData.IsMoving ? movementData.CurrentSpeed : 0f;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, movementConfig.MovementSmoothing);
            animator.SetFloat(SpeedId, currentSpeed);
        }

        public void UpdateFalling()
        {
            var isGrounded = collisionData.OnGrounded;
            var verticalVelocity = movementData.VerticalVelocity;
            var isFalling = !isGrounded && verticalVelocity < 0f;
            animator.SetBool(IsFallingId, isFalling);
        }

        public void UpdateJump()
        {
            var isGrounded = collisionData.OnGrounded;
            var verticalVelocity = movementData.VerticalVelocity;

            animator.SetBool(IsGroundedId, isGrounded);

            if (isGrounded && !wasGrounded)
                animator.SetTrigger(LandId);

            var isJumping = !isGrounded && verticalVelocity > 0f;
            var isFalling = !isGrounded && verticalVelocity < 0f;

            animator.SetBool(IsJumpingId, isJumping);
            animator.SetBool(IsFallingId, isFalling);
            animator.SetFloat(VerticalVelocityId, verticalVelocity);

            wasGrounded = isGrounded;
        }

        public void UpdateCrouch() => animator.SetBool(IsCrouchingId, movementData.IsCrouching);

        public void UpdateSwimming()
        {
            var isSwimming = movementData.IsSwimming;

            animator.SetBool(IsSwimmingId, isSwimming);

            if (isSwimming)
            {
                var swimSpeed = movementData.IsMoving ? Mathf.Abs(movementData.CurrentSpeed) : 0f;
                animator.SetFloat(SwimSpeedId, swimSpeed);
            }

            if (isSwimming && !wasSwimming)
                animator.SetTrigger(EnterWaterId);
            else if (!isSwimming && wasSwimming)
                animator.SetTrigger(ExitWaterId);

            wasSwimming = isSwimming;
        }

        public void UpdateClimbing()
        {
            var isClimbing = movementData.IsClimbing;

            animator.SetBool(IsClimbingId, isClimbing);

            if (isClimbing)
            {
                var climbSpeed = Mathf.Abs(movementData.VerticalVelocity);
                animator.SetFloat(ClimbSpeedId, climbSpeed);
            }

            if (isClimbing && !wasClimbing)
                animator.SetTrigger(StartClimbingId);
            else if (!isClimbing && wasClimbing)
                animator.SetTrigger(StopClimbingId);

            wasClimbing = isClimbing;
        }

        public void TriggerAction(string actionName) => animator.SetTrigger(actionName);

        public void ResetActionTrigger(string actionName) => animator.ResetTrigger(actionName);
    }
}
