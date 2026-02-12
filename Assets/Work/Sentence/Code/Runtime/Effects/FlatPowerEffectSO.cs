using UnityEngine;

namespace Work.Sentence.Code.Runtime.Effects
{
    [CreateAssetMenu(fileName = "FlatPowerEffect", menuName = "SO/Sentence/Effect/FlatPower", order = 20)]
    public class FlatPowerEffectSO : SentenceEffectSO
    {
        [SerializeField] private int amount = 10;

        public override void Build(ref SentenceEffectBuildContext context)
        {
            context.FlatPower += amount;
        }
    }
}

