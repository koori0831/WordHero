using System.Collections.Generic;
using Code.Entities;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.StatSystem.Code;
using Work.Sentence.Code.Runtime;

namespace Work.Sentence.Code.Runtime.Effects
{
    [CreateAssetMenu(fileName = "MoveSpeedTimedBoostEffect", menuName = "SO/Sentence/Effect/MoveSpeedTimedBoost", order = 23)]
    public class MoveSpeedTimedBoostEffectSO : SentenceEffectSO
    {
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private float addAmount = 0.25f;
        [SerializeField] private float durationSeconds = 2f;
        [SerializeField] private bool refreshOnRetrigger = true;
        [SerializeField] private int maxStacks = 1;

        private static readonly Dictionary<long, object> KeyCache = new Dictionary<long, object>(32);

        public override void Fire(in SentenceEffectFireContext context)
        {
            if (moveSpeedStat == null || context.Owner == null) return;

            Entity ownerEntity = context.Owner.GetComponent<Entity>();
            if (ownerEntity == null) return;

            object key = GetOrCreateKey(GetInstanceID(), context.BodyPart);
            StatModifierSpec spec = StatModifierSpec.Add(
                addAmount,
                maxStacks,
                durationSeconds,
                refreshOnRetrigger);

            Bus<StatApplyModifierEvent>.Raise(new StatApplyModifierEvent(ownerEntity, moveSpeedStat, key, spec));
        }

        private static object GetOrCreateKey(int effectInstanceId, BodyPart bodyPart)
        {
            long cacheKey = ((long)effectInstanceId << 32) | (uint)(int)bodyPart;
            if (KeyCache.TryGetValue(cacheKey, out object key))
            {
                return key;
            }

            KeyToken newKey = new KeyToken(effectInstanceId, bodyPart);
            KeyCache.Add(cacheKey, newKey);
            return newKey;
        }

        private sealed class KeyToken
        {
            public readonly int EffectId;
            public readonly BodyPart BodyPart;

            public KeyToken(int effectId, BodyPart bodyPart)
            {
                EffectId = effectId;
                BodyPart = bodyPart;
            }
        }
    }
}

