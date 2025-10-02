using System;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable, IHealable, IDeathable
    {
        [SerializeField]
        int maxHealth = 100;
        int currentHealth;

        public event Action<int> OnHealthDecreased;
        public event Action<int> OnHealthIncreased;
        public event Action OnDeath;

        void Start() => currentHealth = maxHealth;

        public void TakeDamage(int damage)
        {
            OnHealthDecreased?.Invoke(damage);
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
        }

        public void TakeHeal(int cure)
        {
            OnHealthIncreased?.Invoke(cure);
            currentHealth += cure;
            currentHealth = Mathf.Max(0, currentHealth);
            if (currentHealth > 0)
                Die();
        }

        public void Die()
        {
            OnDeath?.Invoke();
            currentHealth -= maxHealth;
            currentHealth = Mathf.Max(0, currentHealth);
            gameObject.SetActive(false);
        }
    }
}
