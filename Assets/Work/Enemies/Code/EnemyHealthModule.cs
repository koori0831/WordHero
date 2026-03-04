using System;
using UnityEngine;
using UnityEngine.Events;
using Work.Core.Utils.EventBus;
using Work.Cursor.Code;

namespace Work.Enemies.Code
{
    public class HpValue
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public Action<int, int> OnHpChanged;
        public Action OnDead;

        public HpValue(int maxHp)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }

        public void SetHp(int maxHp, int currentHp)
        {
            MaxHp = maxHp;
            CurrentHp = currentHp;
        }

        public void Update(int currentHp)
        {
            CurrentHp = currentHp;
        }
    }

    public class EnemyHealthModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;
        public int CurrentHealth
        {
            get => HpValue.CurrentHp;
            private set
            {
                HpValue.Update(value);
            }
        }

        public HpValue HpValue { get; private set; }

        public UnityEvent<int, int> OnHealthChanged;
        public UnityEvent OnDeath;

        [field: SerializeField] public int MaxHealth { get; private set; } = 100;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            HpValue = new HpValue(MaxHealth);
        }


        public void TakeDamage(int damageAmount)
        {
            _owner.StateChangeChannel.SendEventMessage(EnemyState.Hit);
            Bus<EnemyHitEvent>.Raise(new EnemyHitEvent(_owner.gameObject,_owner.InfoData));
            int previousHealth = CurrentHealth;
            CurrentHealth -= damageAmount;
            OnHealthChanged?.Invoke(previousHealth, CurrentHealth);
            HpValue.OnHpChanged?.Invoke(CurrentHealth, MaxHealth);
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDeath?.Invoke();
            HpValue.OnDead?.Invoke();
        }
    }
}