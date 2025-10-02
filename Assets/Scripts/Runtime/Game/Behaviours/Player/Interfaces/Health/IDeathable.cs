using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IDeathable
    {
        event Action<PlayerStatusData> OnDeathStatus;

        void Die();
    }
}
