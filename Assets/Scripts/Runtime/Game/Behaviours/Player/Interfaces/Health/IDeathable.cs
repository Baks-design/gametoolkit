using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IDeathable
    {
        event Action OnDeath;

        void Die();
    }
}
