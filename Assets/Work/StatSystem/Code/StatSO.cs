using System;
using System.Collections.Generic;
using UnityEngine;

namespace Work.StatSystem.Code
{
    [CreateAssetMenu(fileName = "StatSO", menuName = "SO/StatSystem/Stat", order = 0)]
    public class StatSO : ScriptableObject, ICloneable
    {
        public delegate void ValueChangeHandler(StatSO stat, float currentValue, float previousValue);
        public event ValueChangeHandler OnValueChange;

        public string statName;
        public string displayName;
        public float incrementStep = 1f;

        [SerializeField] private float baseValue;
        [field: SerializeField] public float MinValue { get; set; }
        [field: SerializeField] public float MaxValue { get; set; }

        [field: SerializeField] public bool IsPercent { get; private set; }

        // key -> modifier bucket (스택/시간/연산)
        private readonly Dictionary<object, ModifierEntry> _mods = new();

        // 캐시
        private bool _dirty = true;
        private float _cachedValue;

        public float Value
        {
            get
            {
                if (_dirty) RecalculateCache();
                return _cachedValue;
            }
        }

        public bool IsMax => Mathf.Approximately(Value, MaxValue);
        public bool IsMin => Mathf.Approximately(Value, MinValue);

        public float BaseValue
        {
            get => baseValue;
            set
            {
                float previous = Value; // 캐시 반영 포함
                baseValue = Mathf.Clamp(value, MinValue, MaxValue);
                MarkDirtyAndNotify(previous);
            }
        }

        public bool CanIncrementStep() => BaseValue + incrementStep <= MaxValue;

        /// <summary>
        /// key 중복이면 "무시"가 아니라 "스택"으로 동작.
        /// 스택이 싫으면, unique key를 매번 새 객체로 넣으면 됨.
        /// </summary>
        public void AddModifier(object key, float value)
        {
            AddModifier(key, StatModifierSpec.Add(value));
        }

        /// <summary>
        /// key가 같으면 같은 버프로 취급해서 스택 증가.
        /// </summary>
        public void AddModifier(object key, StatModifierSpec spec)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            float previous = Value;

            if (_mods.TryGetValue(key, out var entry))
            {
                // 스택 증가
                int oldStacks = entry.Stacks;
                int maxStacks = Mathf.Max(1, spec.MaxStacks);

                entry.Stacks = Mathf.Min(entry.Stacks + 1, maxStacks);

                // duration 갱신
                if (spec.IsTimed && spec.RefreshDurationOnStack && spec.DurationSeconds.HasValue)
                {
                    entry.RemainingSeconds = Mathf.Max(0f, spec.DurationSeconds.Value);
                }

                // spec이 달라질 여지가 있으면 "최신 spec으로 덮어쓰기"
                entry.Spec = spec;

                _mods[key] = entry;

                // 스택이 max로 막혀서 변화가 없을 수도 있으니, 값이 변했는지로 이벤트 판단
                MarkDirtyAndNotify(previous);
                return;
            }

            var newEntry = new ModifierEntry(spec);
            _mods.Add(key, newEntry);

            MarkDirtyAndNotify(previous);
        }

        public void RemoveModifier(object key)
        {
            if (key == null) return;
            if (_mods.Count == 0) return;

            float previous = Value;

            if (_mods.Remove(key))
            {
                MarkDirtyAndNotify(previous);
            }
        }

        public void ClearModifiers()
        {
            if (_mods.Count == 0) return;

            float previous = Value;
            _mods.Clear();
            MarkDirtyAndNotify(previous);
        }

        /// <summary>
        /// 시간제 modifier 만료 처리.
        /// dt만큼 흘리고, 만료된 것 제거.
        /// return: 값 변화(또는 만료로 인한 dirty) 발생 여부
        /// </summary>
        public bool Tick(float deltaTime)
        {
            if (_mods.Count == 0) return false;
            if (deltaTime <= 0f) return false;

            bool removedAny = false;

            // 만료 제거(역순 루프 위해 임시 리스트 사용)
            // key 컬렉션을 직접 수정 못 하니, 제거 대상만 모았다가 제거
            List<object> toRemove = null;

            foreach (var kv in _mods)
            {
                var key = kv.Key;
                var entry = kv.Value;

                if (!entry.Spec.IsTimed) continue;

                entry.RemainingSeconds -= deltaTime;
                if (entry.RemainingSeconds <= 0f)
                {
                    toRemove ??= new List<object>(4);
                    toRemove.Add(key);
                }
                else
                {
                    // 남은시간 갱신 반영
                    _mods[key] = entry;
                }
            }

            if (toRemove != null)
            {
                float previous = Value;

                for (int i = 0; i < toRemove.Count; i++)
                    removedAny |= _mods.Remove(toRemove[i]);

                if (removedAny)
                    MarkDirtyAndNotify(previous);
            }

            return removedAny;
        }

        private void MarkDirtyAndNotify(float previousValue)
        {
            _dirty = true;
            RecalculateCache(); // 이벤트 비교하려고 즉시 재계산
            TryInvokeValueChange(_cachedValue, previousValue);
        }

        private void RecalculateCache()
        {
            // 계산 파이프라인: (base + AddSum) * MulProd
            float addSum = 0f;
            float mulProd = 1f;

            foreach (var entry in _mods.Values)
            {
                int stacks = Mathf.Max(1, entry.Stacks);
                var spec = entry.Spec;

                if (spec.Op == StatModOp.Add)
                {
                    addSum += spec.ValuePerStack * stacks;
                }
                else if (spec.Op == StatModOp.Mul)
                {
                    // "스택당 선형"으로: (1 + x*stacks)
                    mulProd *= (1f + spec.ValuePerStack * stacks);
                }
            }

            float raw = (baseValue + addSum) * mulProd;
            _cachedValue = Mathf.Clamp(raw, MinValue, MaxValue);
            _dirty = false;
        }

        private void TryInvokeValueChange(float currentValue, float previousValue)
        {
            if (!Mathf.Approximately(currentValue, previousValue))
                OnValueChange?.Invoke(this, currentValue, previousValue);
        }

        /// <summary>
        /// Clone 결과가 modifier 상태를 공유하지 않도록 런타임 상태 초기화.
        /// </summary>
        public object Clone()
        {
            var cloned = Instantiate(this);
            cloned.ResetRuntimeState();
            return cloned;
        }

        private void ResetRuntimeState()
        {
            _mods.Clear();
            _dirty = true;
            _cachedValue = Mathf.Clamp(baseValue, MinValue, MaxValue);
        }

        private struct ModifierEntry
        {
            public StatModifierSpec Spec;
            public int Stacks;
            public float RemainingSeconds;

            public ModifierEntry(StatModifierSpec spec)
            {
                Spec = spec;
                Stacks = 1;
                RemainingSeconds = spec.DurationSeconds.HasValue ? Mathf.Max(0f, spec.DurationSeconds.Value) : 0f;
            }
        }
    }
}