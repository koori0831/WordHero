using UnityEngine;

namespace Work.Sentence.Code.Runtime.Triggers
{
    [CreateAssetMenu(fileName = "OnHitTrigger", menuName = "SO/Sentence/Trigger/OnHit", order = 11)]
    public class OnHitTriggerSO : SentenceTriggerSO
    {
        public override ISentenceTriggerRuntime CreateRuntime(GameObject owner)
        {
            return new OnHitTriggerRuntime(owner);
        }
    }
}

