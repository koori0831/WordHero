using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Work.Combat.Code;
using Work.Enemies.Code;
using Work.Entities;
using Work.Information.Code;

namespace Work.Agents.Code
{
    public abstract class Agent : MonoBehaviour,IDamageable, IKnockbackable, IInformationable
    {
        public UnityEvent<int> OnHitEvent;
        public UnityEvent<KnockbackData> OnKnockbackEvent;
        public Transform Transform => gameObject != null ? transform : null;
        public bool IsDead { get; protected set; } = false;
        public InfoDataSO InfoData { get; protected set; }
        protected Dictionary<Type, IAgentModule> _modules = new Dictionary<Type, IAgentModule>();

        public virtual void Init()
        {
            AddModule();
            ModuleInit();
            ModuleAfterInit();
        }

        public virtual void ModuleAfterInit()
        {
            foreach (var module in _modules.Values)
            {
                if (module is IAfterInit afterInitModule)
                {
                    afterInitModule.AfterInitialize();
                }
            }

            
        }

        protected void ModuleInit()
        {
            foreach (var module in _modules.Values)
            {
                module.Initialize(this);
            }
        }

        protected void AddModule()
        {
            _modules = GetComponentsInChildren<IAgentModule>(true).ToList().ToDictionary(item => item.GetType());

            string m = $"이름 : {name} \n";
            foreach (var kvp in _modules)
            {

                m += $"{kvp.Value.GetType().ToString()} \n";
            }
            Debug.Log(m);
        }

        public T GetModule<T>(bool isAssignable = false) where T : class, IAgentModule
        {
            if (_modules.TryGetValue(typeof(T), out var compo))
                return compo as T;
            if (isAssignable == false)
            {
                Debug.LogError($"Not Find {typeof(T)}");
                return null;
            }

            foreach (var kvp in _modules)
            {
                if (kvp.Value is T tComp)
                    return tComp;
            }

            Debug.LogError($"Not Find {typeof(T)}");
            return null;
        }

        public virtual void InitInfo(InfoDataSO infoData)
        {
            InfoData = infoData;
        }

        public virtual void TakeKnockback(KnockbackData knockbackData)
        {
            if (IsDead) return;
            OnKnockbackEvent?.Invoke(knockbackData);
        }

        public virtual void TakeDamage(int damageAmount)
        {
            OnHitEvent?.Invoke(damageAmount);
        }

        public virtual void Die()
        {
            if (IsDead) return;
            IsDead = true;
        }
    }
}