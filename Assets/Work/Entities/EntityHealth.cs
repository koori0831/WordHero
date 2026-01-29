using UnityEngine;
using System;
using Work.StatSystem.Code;

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
    public class EntityHealth : MonoBehaviour, IEntityComponent, IAfterInitCompo
    {
        [SerializeField] private StatSO statSO;

        private Entity _entity;
        private EntityStatCompo _stat;
        private StatSO _healthStat;

        public Entity Owner => _entity;

        public int MaxHP => (int)_healthStat.Value;

        [field: SerializeField] public int Health { get; private set; }

        public bool IsDamageImmune { get; set; } = false;

        public event Action DeadTrigger;
        public event Action<HealthData> HealthChangedTrigger;
        public event Action DamagedTrigger;

        public void InitCompo(Entity entity)
        {
            _entity = entity;
            _stat = entity.GetCompo<EntityStatCompo>();
        }

        public void AfterInit()
        {
            _healthStat = _stat.GetStat(statSO);
            Health = MaxHP;
        }

        public void IncreaseHP(int value)
        {
            Health = Math.Min(Health + value, MaxHP);
            HealthData healthData = new HealthData(Health, MaxHP);
            HealthChangedTrigger?.Invoke(healthData);
        }

        public void DecreaseHP(int value)
        {
            if (IsDamageImmune) return;

            Health = Math.Max(0, Health - value);
            HealthData healthData = new HealthData(Health, MaxHP);
            HealthChangedTrigger?.Invoke(healthData);
            if (Health == 0)
            {
                DeadTrigger?.Invoke();
                return;
            }
            DamagedTrigger?.Invoke();
        }
    }
}