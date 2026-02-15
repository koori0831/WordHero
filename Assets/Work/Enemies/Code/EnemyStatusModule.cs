using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Work.Combat.Code;

namespace Work.Enemies.Code
{
    public class EnemyStatusModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _enemy;

        private Queue<StatusEffect> _status = new Queue<StatusEffect>();

        private Dictionary<StatusType, StatusEffect> _activeEffects = new Dictionary<StatusType, StatusEffect>();
        private List<StatusType> _tickEffects = new List<StatusType>();
        private List<StatusType> _removeEffects = new List<StatusType>();

        public void Initialize(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void AddStatus(StatusEffect statusEffect)
        {
            _status.Enqueue(statusEffect);
        }

        public void Update()
        {
            ProcessStatusEffects();
            UpdateStatusEffects();
            UpdateTickEffects();
            UpdateRemoveEffects();
        }

        private void UpdateRemoveEffects()
        {
            for(int i = 0; i < _removeEffects.Count; i++)
            {
                if (_activeEffects.ContainsKey(_removeEffects[i]))
                {
                    // 효과 제거 로직 (예: 스턴 해제, 화상 해제 등)
                    Debug.Log($"Removed status effect: {_removeEffects[i]}");
                    _activeEffects.Remove(_removeEffects[i]);
                }
            }
            _removeEffects.Clear();
        }
        private void UpdateTickEffects()
        {
            for(int i = 0; i < _tickEffects.Count; i++)
            {
                ApplyTickEffect(_tickEffects[i]);
            }

            _tickEffects.Clear();
        }

        public void ProcessStatusEffects()
        {
            while (_status.Count > 0)
            {
                var effect = _status.Dequeue();
                ApplyStatusEffect(effect);
            }
        }
        private void UpdateStatusEffects()
        {
            foreach (StatusEffect item in _activeEffects.Values)
            {
                if (!item.isInfinite)
                {
                    item.timer += Time.deltaTime;
                    if (item.Duration <= item.timer)
                    {
                        _removeEffects.Add(item.type);
                    }

                    item.tickTimer += Time.deltaTime;
                    if (item.TickInterval <= item.tickTimer)
                    {
                        item.tickTimer = 0;
                        _tickEffects.Add(item.type);
                    }
                }
            }
        }

        private void ApplyStatusEffect(StatusEffect effect)
        {
            if (_activeEffects.ContainsKey(effect.type))
            {
                // 이미 같은 타입의 효과가 존재하면 갱신하거나 중첩 처리
                _activeEffects[effect.type] = effect; // 간단히 갱신하는 예시
            }
            else
            {
                _activeEffects.Add(effect.type, effect);
                // 효과 적용 로직 (예: 스턴, 화상 등)
                Debug.Log($"Applied status effect: {effect.type}");
            }
        }
        public void ApplyTickEffect(StatusType type)
        {
            switch (type)
            {
                case StatusType.Burn:
                    break;
                case StatusType.Poison:
                    break;
                case StatusType.Bleed:
                    break;
                case StatusType.Shock:
                    break;
            }
        }

        public bool HasStatusEffect(StatusType type)
        {
            return _activeEffects.ContainsKey(type);
        }
    }
}