using UnityEngine;

namespace Work.Sentence.Code.Runtime
{
    public readonly struct SentenceTriggerSignal
    {
        public readonly GameObject Source;
        public readonly GameObject Target;
        public readonly int Damage;
        public readonly bool IsCritical;

        public SentenceTriggerSignal(GameObject source, GameObject target, int damage, bool isCritical)
        {
            Source = source;
            Target = target;
            Damage = damage;
            IsCritical = isCritical;
        }
    }
}

