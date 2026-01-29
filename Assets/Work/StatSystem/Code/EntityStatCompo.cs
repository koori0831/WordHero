using Code.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.StatSystem.Code
{
    public class EntityStatCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private StatOverride[] statOverrides;
        [SerializeField] private bool useLocalTick = true;
        [SerializeField] private bool useUnscaledTime = false;

        private Dictionary<string, StatSO> _stats;
        public Entity Owner { get; private set; }

        public void InitCompo(Entity entity)
        {
            Owner = entity;
            _stats = statOverrides.ToDictionary(
                s => s.stat.statName,
                stat => stat.CreateStat());

            Bus<StatApplyModifierEvent>.Events += OnApply;
            Bus<StatRemoveModifierEvent>.Events += OnRemove;
        }

        public void OnDisable()
        {
            Bus<StatApplyModifierEvent>.Events -= OnApply;
            Bus<StatRemoveModifierEvent>.Events -= OnRemove;
        }

        private void Update()
        {
            if (!useLocalTick || _stats == null) return;

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            foreach (StatSO stat in _stats.Values)
                stat.Tick(dt);
        }


        public StatSO GetStat(StatSO stat)
        {
            Debug.Assert(stat != null, "StatSO cannot be null.");
            return _stats.GetValueOrDefault(stat.statName);
        }

        public bool TryGetStat(StatSO stat, out StatSO outStat)
        {
            Debug.Assert(stat != null, "StatSO cannot be null.");
            outStat = _stats.GetValueOrDefault(stat.statName);
            return outStat != null;
        }

        public bool TryGetStat(string statName, out StatSO outStat)
        {
            outStat = _stats.GetValueOrDefault(statName);
            return outStat != null;
        }

        public void SetBaseValue(StatSO stat, float value)
        {
            StatSO targetStat = GetStat(stat);
            Debug.Assert(targetStat != null, $"StatSO '{stat.statName}' not found in EntityStatCompo.");
            targetStat.BaseValue = value;
        }

        public float GetBaseValue(StatSO stat)
        {
            StatSO targetStat = GetStat(stat);
            Debug.Assert(targetStat != null, $"StatSO '{stat.statName}' not found in EntityStatCompo.");
            return targetStat.BaseValue;
        }

        public void IncreaseBaseValue(StatSO stat, float value)
        {
            StatSO targetStat = GetStat(stat);
            Debug.Assert(targetStat != null, $"StatSO '{stat.statName}' not found in EntityStatCompo.");
            targetStat.BaseValue += value;
        }

        public void AddModifier(StatSO stat, object key, float value)
        {
            StatSO targetStat = GetStat(stat);
            Debug.Assert(targetStat != null, $"StatSO '{stat.statName}' not found in EntityStatCompo.");
            targetStat.AddModifier(key, value);
        }

        public void RemoveModifier(StatSO stat, object key)
        {
            StatSO targetStat = GetStat(stat);
            Debug.Assert(targetStat != null, $"StatSO '{stat.statName}' not found in EntityStatCompo.");
            targetStat.RemoveModifier(key);
        }

        public void ClearAllModifiers()
        {
            foreach (StatSO stat in _stats.Values)
            {
                stat.ClearModifiers();
            }
        }

        public float SubscribeStat(StatSO stat, StatSO.ValueChangeHandler handler, float defaultValue)
        {
            StatSO targetStat = GetStat(stat);
            if (targetStat == null) return defaultValue;

            targetStat.OnValueChange += handler;
            return targetStat.Value;
        }

        public void UnSubscribeStat(StatSO stat, StatSO.ValueChangeHandler handler)
        {
            StatSO targetStat = GetStat(stat);
            if (targetStat == null) return;
            targetStat.OnValueChange -= handler;
        }

        private void OnApply(StatApplyModifierEvent e)
        {
            if (e.Target != Owner) return;
            StatSO runtime = GetStat(e.Stat);
            if (runtime != null) runtime.AddModifier(e.Key, e.Spec);
        }

        private void OnRemove(StatRemoveModifierEvent e)
        {
            if (e.Target != Owner) return;
            StatSO runtime = GetStat(e.Stat);
            if (runtime != null) runtime.RemoveModifier(e.Key);
        }

    }
}