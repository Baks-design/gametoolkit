using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAnimationController : MonoBehaviour, IUpdatable
    {
        IUpdateServices updateServices;

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

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

        void OnDisable() => updateServices.Unregister(this);
    }
}
