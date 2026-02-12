using UnityEngine;

namespace Work.Sentence.Code.Runtime.Triggers
{
    [CreateAssetMenu(fileName = "OnCritTrigger", menuName = "SO/Sentence/Trigger/OnCrit", order = 10)]
    public class OnCritTriggerSO : SentenceTriggerSO
    {
        public override ISentenceTriggerRuntime CreateRuntime(GameObject owner)
        {
            return new OnCritTriggerRuntime(owner);
        }
    }
}

