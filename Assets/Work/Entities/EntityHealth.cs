using UnityEngine;
using System;

namespace Code.Entities
{
    public struct HealthData
    {
        public int CurrentHealth;
        public int MaxHealth;
        public HealthData(int currentHealth, int maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }

    [RequireComponent(typeof(Collider))]
    public class EntityHealth : MonoBehaviour, IEntityComponent
    {
        private Entity _entity;

        public Entity Owner => _entity;

        [SerializeField] private int maxHP = 100;

        [field: SerializeField] public int Health { get; private set; }

        public bool IsDamageImmune { get; set; } = false;

        public event Action DeadTrigger;
        public event Action<HealthData> HealthChangedTrigger;
        public event Action DamagedTrigger;

        public void InitCompo(Entity entity)
        {
            _entity = entity;
            Health = maxHP;
        }

        public void IncreaseHP(int value)
        {
            Health = Math.Min(Health + value, maxHP);
            HealthData healthData = new HealthData(Health, maxHP);
            HealthChangedTrigger?.Invoke(healthData);
        }

        public void IncreaseMaxHP(int value)
        {
            maxHP += value;
            HealthData healthData = new HealthData(Health, maxHP);
        }

        public void DecreaseHP(int value)
        {
            if (IsDamageImmune) return;

            Health = Math.Max(0, Health - value);
            HealthData healthData = new HealthData(Health, maxHP);
            HealthChangedTrigger?.Invoke(healthData);
            if (Health == 0)
            {
                DeadTrigger?.Invoke();
                return;
            }
            DamagedTrigger?.Invoke();
        }

        public void DecreaseMaxHP(int value)
        {
            maxHP = Math.Max(1, maxHP - value);
            if (Health > maxHP)
            {
                Health = maxHP;
            }
            HealthData healthData = new HealthData(Health, maxHP);
            HealthChangedTrigger?.Invoke(healthData);
            DamagedTrigger?.Invoke();
        }
    }
}