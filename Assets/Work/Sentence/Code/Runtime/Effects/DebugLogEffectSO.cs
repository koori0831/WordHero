using UnityEngine;

namespace Work.Sentence.Code.Runtime.Effects
{
    [CreateAssetMenu(fileName = "DebugLogEffect", menuName = "SO/Sentence/Effect/DebugLog", order = 22)]
    public class DebugLogEffectSO : SentenceEffectSO
    {
        [SerializeField] private string message = "Sentence triggered.";

        public override void Fire(in SentenceEffectFireContext context)
        {
            int finalValue = Mathf.RoundToInt(context.FlatPower * context.Multiplier);
            Debug.Log($"{message} Part={context.BodyPart}, FinalValue={finalValue}, Crit={context.Signal.IsCritical}");
        }
    }
}

