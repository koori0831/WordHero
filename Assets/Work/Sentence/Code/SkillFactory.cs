using UnityEngine;
using System.Collections;

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

            return inst;
        }
    }
}