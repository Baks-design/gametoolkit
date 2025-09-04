using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public struct PlayerInputData
    {
        public Vector2 MoveInput;
        public bool HasMoveInput;
        public Vector2 LookInput;
        public bool RunPressed;
        public bool RunReleased;
        public bool JumpPressed;
        public bool ZoomPressed;
        public bool ZoomReleased;

        public bool IsCrouching;
        public bool IsRunning;
        public bool IsZooming;
    }
}
