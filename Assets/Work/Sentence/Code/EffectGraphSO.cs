using UnityEngine;
using System.Collections;
using Work.StatSystem.Code;

namespace Work.Sentence.Code
{
    [CreateAssetMenu(menuName = "SO/Effects/Effect Graph (MVP)")]
    public class EffectGraphSO : ScriptableObject
    {
        public SkillKind Kind = SkillKind.Attack;

        [Header("Common")]
        public float BaseCooldown = 1.0f;

        [Header("Attack")]
        public float BaseDamage = 10f;

        [Header("Buff/CC")]
        public float BaseDuration = 2.0f;
        public float Magnitude = 0.2f; // 예: 공격력 +20%, 캐스팅시간 -20% 등

        [Header("Stat Modifier (optional)")]
        public bool ApplyStatModifier = false;
        public StatSO TargetStat;
        public StatModOp StatOp = StatModOp.Add;
        public float StatAmount = 1f;
        public int StatMaxStacks = 1;
        public bool StatRefreshDurationOnStack = true;
    }
}
