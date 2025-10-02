using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IPlayerCamera
    {
        void BreathingHandler(float deltaTime);
        void RotationHandler(float deltaTime);
        void SwayHandler(Vector3 inputVector, float rawXInput, float deltaTime);
        void AimHandler(float deltaTime);
        void RunFOVHandler(bool returning, float deltaTime);
    }
}
