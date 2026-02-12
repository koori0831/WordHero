using UnityEngine;

namespace Work.Sentence.Code.Runtime.Triggers
{
    [CreateAssetMenu(fileName = "OnDodgeTrigger", menuName = "SO/Sentence/Trigger/OnDodge", order = 13)]
    public class OnDodgeTriggerSO : SentenceTriggerSO
    {
        public override ISentenceTriggerRuntime CreateRuntime(GameObject owner)
        {
            return new OnDodgeTriggerRuntime(owner);
        }
    }
}

