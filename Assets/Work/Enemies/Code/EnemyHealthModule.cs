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

        public UnityEvent<int,int> OnHealthChanged;
        public UnityEvent OnDeath;

        [field:SerializeField] public int MaxHealth { get; private set; } = 100;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            currentHealth = MaxHealth;
        }


        public void TakeDamage(int damageAmount)
        {
            int previousHealth = currentHealth;
            currentHealth -= damageAmount;
            OnHealthChanged?.Invoke(previousHealth, currentHealth);
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