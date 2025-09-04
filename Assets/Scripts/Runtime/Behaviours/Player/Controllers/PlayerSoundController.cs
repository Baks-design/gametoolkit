using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerSoundController : CustomMonoBehaviour
    {
        [SerializeField]
        PlayerMovementController movementController;

        public override void ProcessUpdate(float deltaTime)
        {
            base.ProcessUpdate(deltaTime);
        }
    }
}
