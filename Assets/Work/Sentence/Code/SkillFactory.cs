using UnityEngine;
using System.Collections;
using Work.StatSystem.Code;

namespace Work.Sentence.Code
{
    public sealed class SkillFactory
    {
        public SkillInstance Create(ResolvedSentence r)
        {
            var e = r.Template.Effect;
            var draft = r.Draft;

            var inst = new SkillInstance
            {
                Kind = e.Kind,
                DebugName = r.Template.name,
                Cooldown = e.BaseCooldown,
                Damage = e.BaseDamage,
                Duration = e.BaseDuration,
                Magnitude = e.Magnitude,
                Tags = draft.GetMergedTags()
            };

            // Duration 토큰이 있으면 duration을 덮어쓰기(또는 곱하기 정책 택1)
            var dur = draft.GetDurationTokenOrNone();
            if (dur.HasValue)
                inst.Duration = Mathf.Max(0.1f, dur.Value);

            // Option Modifier 예시(가볍게)
            if (draft.Option != null)
            {
                var m = draft.Option.Tags.Modifier;
                if ((m & ModifierTag.PowerUp) != 0) inst.Damage *= 1.25f;
                if ((m & ModifierTag.CastTimeDown) != 0) inst.Cooldown *= 0.85f;
            }

            // 정문 보너스(작게)
            if (r.ProperBonus)
            {
                inst.Damage *= 1.10f;
                // 혹은 Cooldown 감소 등으로 바꿔도 됨
            }

            if (e.ApplyStatModifier)
            {
                var stat = ResolveStatTarget(draft, e);
                if (stat != null)
                {
                    var (op, amount) = ResolveStatChange(draft, e);
                    float? durationSeconds = inst.Duration > 0f ? inst.Duration : null;
                    var spec = op == StatModOp.Add
                        ? StatModifierSpec.Add(amount, e.StatMaxStacks, durationSeconds, e.StatRefreshDurationOnStack)
                        : StatModifierSpec.Mul(amount, e.StatMaxStacks, durationSeconds, e.StatRefreshDurationOnStack);

                    inst.HasStatEffect = true;
                    inst.StatEffect = new StatEffectSpec
                    {
                        Stat = stat,
                        Modifier = spec,
                        Key = $"{inst.DebugName}:{stat.statName}"
                    };
                }
            }

            return inst;
        }

        private static StatSO ResolveStatTarget(SentenceDraft draft, EffectGraphSO effect)
        {
            if (draft.Object != null && draft.Object.LinkedStat != null)
                return draft.Object.LinkedStat;

            if (draft.Subject != null && draft.Subject.LinkedStat != null)
                return draft.Subject.LinkedStat;

            return effect.TargetStat;
        }

        private static (StatModOp op, float amount) ResolveStatChange(SentenceDraft draft, EffectGraphSO effect)
        {
            var token = draft.GetChangeTokenOrNone();
            if (token.HasValue)
            {
                if (token.Kind == NumericKind.Percent)
                    return (StatModOp.Mul, token.Value / 100f);
                if (token.Kind == NumericKind.Flat)
                    return (StatModOp.Add, token.Value);
            }

            return (effect.StatOp, effect.StatAmount);
        }
    }
}
