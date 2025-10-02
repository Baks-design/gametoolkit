using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IDamageable
    {
        event Action<int> OnHealthDecreased;

        void TakeDamage(int damage);
    }
}
