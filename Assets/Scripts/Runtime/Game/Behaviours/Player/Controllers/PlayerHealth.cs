using System;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerHealth : MonoBehaviour, IHealthChangeable, IDeathable
    {
        [SerializeField]
        int initialMaxHealth = 100;
        PlayerStatus status;

        public event Action<PlayerStatusData> OnHealthChanged;
        public event Action<int, int> OnHealthAmountChanged;
        public event Action<PlayerStatusData> OnDeathStatus;

        void Awake()
        {
            var initialStatus = new PlayerStatusData
            {
                MaxHealth = initialMaxHealth,
                CurrentHealth = initialMaxHealth
            };
            status = new PlayerStatus(initialStatus);
            status.UpdateStatus(_ => initialStatus);
        }

        void OnEnable()
        {
            status.OnStatusChanged += HandleStatusChanged;
            OnHealthChanged += HandleHealthChanged;
            OnDeathStatus += HandleDeath;
        }

        void OnDisable()
        {
            status.OnStatusChanged -= HandleStatusChanged;
            OnHealthChanged -= HandleHealthChanged;
            OnDeathStatus -= HandleDeath;
        }

        void HandleStatusChanged(PlayerStatusData newStatus, PlayerStatusData previousStatus)
        {
            if (newStatus.CurrentHealth == previousStatus.CurrentHealth)
                return;

            OnHealthChanged?.Invoke(newStatus);
            OnHealthAmountChanged?.Invoke(newStatus.CurrentHealth, newStatus.MaxHealth);

            if (!newStatus.IsAlive && previousStatus.IsAlive)
            {
                OnDeathStatus?.Invoke(newStatus);
                Die();
            }
        }

        void HandleHealthChanged(PlayerStatusData status) { }

        void HandleDeath(PlayerStatusData status) { }

        public void TakeDamage(int damage)
        {
            if (!status.CurrentStatus.IsAlive)
                return;

            status.UpdateStatus(status => status.WithDamage(damage));
        }

        public void TakeHeal(int healAmount)
        {
            if (!status.CurrentStatus.IsAlive)
                return;

            status.UpdateStatus(status => status.WithHealing(healAmount));
        }

        public void Die()
        {
            gameObject.SetActive(false);

            status.UpdateStatus(status => status with { CurrentHealth = 0 });
        }

        public void FullHeal() => status.UpdateStatus(status => status.WithFullHeal());
    }
}
