using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Agents.Code
{
    public class StatusValue
    {
        public Action<StatusType, bool> OnstateusChangeEvent;
        public Action<StatusType> OnStatusTickEvent;
        public bool isHitImmunity;
        public bool isSuperArmor;
        public bool isInvincible;
    }

    public class AgentStatusModule : MonoBehaviour, IAgentModule
    {
        private Agent _agent;

        private Queue<StatusEffect> _status = new Queue<StatusEffect>();

        private Dictionary<StatusType, StatusEffect> _activeEffects = new Dictionary<StatusType, StatusEffect>();
        private List<StatusType> _tickEffects = new List<StatusType>();
        private List<StatusType> _removeEffects = new List<StatusType>();

        public StatusValue StatusValue { get; private set; }

        public void Initialize(Agent agent)
        {
            _agent = agent;
            StatusValue = new StatusValue();
            StatusValue.isSuperArmor = HasStatusEffect(StatusType.SuperArmor);
            StatusValue.isInvincible = HasStatusEffect(StatusType.Invincible);
            StatusValue.isHitImmunity = HasStatusEffect(StatusType.HitImmunity);
        }

        public void AddStatus(StatusEffect statusEffect)
        {
            if (statusEffect == null) return;
            _status.Enqueue(CloneStatus(statusEffect));
        }

        public void RemoveStatus(StatusType type)
        {
            if (!_activeEffects.ContainsKey(type)) return;
            _removeEffects.Add(type);
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
            for (int i = 0; i < _removeEffects.Count; i++)
            {
                if (_activeEffects.ContainsKey(_removeEffects[i]))
                {
                    // 효과 제거 로직 (예: 스턴 해제, 화상 해제 등)
                    Debug.Log($"Removed status effect: {_removeEffects[i]}");
                    if (_removeEffects[i] == StatusType.SuperArmor)
                        StatusValue.isSuperArmor = false;
                    if (_removeEffects[i] == StatusType.Invincible)
                        StatusValue.isInvincible = false;
                    if (_removeEffects[i] == StatusType.HitImmunity)
                        StatusValue.isHitImmunity = false;

                    StatusValue.OnstateusChangeEvent?.Invoke(_removeEffects[i], false); // 효과 제거 이벤트 호출
                    _activeEffects.Remove(_removeEffects[i]);
                }
            }
            _removeEffects.Clear();
        }
        private void UpdateTickEffects()
        {
            for (int i = 0; i < _tickEffects.Count; i++)
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

                if (effect.type == StatusType.SuperArmor)
                    StatusValue.isSuperArmor = true;
                if (effect.type == StatusType.Invincible)
                    StatusValue.isInvincible = true;
                if (effect.type == StatusType.HitImmunity)
                    StatusValue.isHitImmunity = true;

                StatusValue.OnstateusChangeEvent?.Invoke(effect.type, true); // 효과 적용 이벤트 호출
                // 효과 적용 로직 (예: 스턴, 화상 등)
                Debug.Log($"Applied status effect: {effect.type}");
            }
        }
        public void ApplyTickEffect(StatusType type)
        {
            StatusValue.OnStatusTickEvent?.Invoke(type);
        }

        public bool HasStatusEffect(StatusType type)
        {
            return _activeEffects.ContainsKey(type);
        }

        public float GetStatusValue(StatusType type)
        {
            if (_activeEffects.TryGetValue(type, out StatusEffect effect))
            {
                return effect.Value;
            }

            return 0f;
        }

        private static StatusEffect CloneStatus(StatusEffect source)
        {
            string json = JsonUtility.ToJson(source);
            return JsonUtility.FromJson<StatusEffect>(json);
        }
    }
}
