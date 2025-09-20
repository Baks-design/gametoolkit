using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAnimationController : MonoBehaviour, IUpdatable
    {
        void OnEnable() => UpdateManager.Register(this);

        void OnDisable() => UpdateManager.Unregister(this);

        public void ProcessUpdate(float deltaTime)
        {
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
