using UnityEngine;
using Work.Sentence.Code.Runtime.Effects;

namespace Work.Sentence.Code.Data
{
    [CreateAssetMenu(fileName = "ModifierWord", menuName = "SO/Sentence/ModifierWord", order = 1)]
    public class ModifierWordSO : SentenceWordSO
    {
        [SerializeField] private SentenceEffectSO[] effects;

        public SentenceEffectSO[] Effects => effects;
    }
}

