using UnityEngine;

namespace Work.Sentence.Code.Runtime.Effects
{
    public abstract class SentenceEffectSO : ScriptableObject
    {
        public virtual void Build(ref SentenceEffectBuildContext context)
        {
        }

        public virtual void Fire(in SentenceEffectFireContext context)
        {
        }
    }
}

