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
        public Action<int, int> OnHpChanged; // CurrentHp, MaxHp
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

    public class AgentHealthModule : MonoBehaviour, IAgentModule, IAfterInitialize
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

        public HpValue HpValue { get; protected set; }

        public UnityEvent<int, int> OnHealthChanged;
        public UnityEvent OnDeath;

        [field: SerializeField] public int MaxHealth { get; protected set; } = 100;


        public virtual void Initialize(Agent agent)
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

        public void Heal(int healAmount)
        {
            int previousHealth = CurrentHealth;
            CurrentHealth += healAmount;
            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(previousHealth, CurrentHealth);
            HpValue.OnHpChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        protected void Die()
        {
            OnDeath?.Invoke();
            HpValue.OnDead?.Invoke();
        }

        public virtual void AfterInitialize() { }
    }
}
