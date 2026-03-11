using System;
using UnityEngine;
using UnityEngine.Events;
using Work.Enemies.Code;

namespace Work.Agents.Code
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

    public class AgentHealthModule : MonoBehaviour, IAgentModule
    {
        protected Agent _owner;
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


        public void Initialize(Agent agent)
        {
            _owner = agent;
            HpValue = new HpValue(MaxHealth);
        }

        public virtual void TakeDamage(int damageAmount)
        {
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
