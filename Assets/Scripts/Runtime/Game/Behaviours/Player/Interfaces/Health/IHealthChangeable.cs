using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IHealthChangeable
    {
        event Action<PlayerStatusData> OnHealthChanged;
        event Action<int, int> OnHealthAmountChanged;

        void TakeDamage(int damage);
        void TakeHeal(int healAmount);
        void Die();
        void FullHeal();
    }
}
