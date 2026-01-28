using System;
using UnityEngine;
using UnityEngine.Events;

namespace Work.Enemies.Code
{
    public class EnemyHealthModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;
        private int currentHealth;
        public int CurrentHealth => currentHealth;

        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnDeath;

        [SerializeField] private int maxHealth = 100;
        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            currentHealth = maxHealth;
        }


        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            OnHealthChanged?.Invoke(currentHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }
    }
}