using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAnimationController : CustomMonoBehaviour
    {
        [SerializeField]
        Animator animator;

        public override void ProcessUpdate(float deltaTime)
        {
            base.ProcessUpdate(deltaTime);

            UpdateMoving();
            UpdateJump();
            UpdateCrouch();
            UpdateSwimming();
            UpdateClimbing();
        }

        void UpdateMoving() { }

        void UpdateJump() { }

        void UpdateCrouch() { }

        void UpdateSwimming() { }

        void UpdateClimbing() { }
    }
}
