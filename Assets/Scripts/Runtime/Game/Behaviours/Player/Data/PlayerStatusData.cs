using System;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public record PlayerStatusData
    {
        public int CurrentHealth { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public bool IsAlive => CurrentHealth > 0;
        public float HealthPercentage => MaxHealth > 0 ? (float)CurrentHealth / MaxHealth : 0f;

        public PlayerStatusData WithDamage(int damage) =>
            this with
            {
                CurrentHealth = Mathf.Max(0, CurrentHealth - damage)
            };

        public PlayerStatusData WithHealing(int healAmount) =>
            this with
            {
                CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + healAmount)
            };

        public PlayerStatusData WithMaxHealth(int newMaxHealth) =>
            this with
            {
                MaxHealth = newMaxHealth,
                CurrentHealth = Mathf.Min(CurrentHealth, newMaxHealth)
            };

        public PlayerStatusData WithFullHeal() => this with { CurrentHealth = MaxHealth };
    }
}
