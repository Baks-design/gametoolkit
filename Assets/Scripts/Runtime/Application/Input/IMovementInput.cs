using UnityEngine;
using UnityEngine.InputSystem;

namespace GameToolkit.Runtime.Application.Input
{
    public interface IMovementInput
    {
        InputAction OpenMenuPressed();
        Vector2 GetLook();
        bool AimPressed();
        bool AimReleased();
        Vector2 GetMovement();
        bool HasMovement();
        bool JumpPressed();
        bool CrouchPressed();
        bool SprintPressed();
        bool SprintReleased();
    }
}
