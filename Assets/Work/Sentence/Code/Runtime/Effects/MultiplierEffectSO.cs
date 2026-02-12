using UnityEngine;

namespace Work.Sentence.Code.Runtime.Effects
{
    [CreateAssetMenu(fileName = "MultiplierEffect", menuName = "SO/Sentence/Effect/Multiplier", order = 21)]
    public class MultiplierEffectSO : SentenceEffectSO
    {
        [SerializeField] private float amount = 0.1f;

        public override void Build(ref SentenceEffectBuildContext context)
        {
            context.Multiplier += amount;
        }
    }
}

