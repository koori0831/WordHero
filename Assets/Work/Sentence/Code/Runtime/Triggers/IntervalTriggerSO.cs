using UnityEngine;

namespace Work.Sentence.Code.Runtime.Triggers
{
    [CreateAssetMenu(fileName = "IntervalTrigger", menuName = "SO/Sentence/Trigger/Interval", order = 12)]
    public class IntervalTriggerSO : SentenceTriggerSO
    {
        [SerializeField] private float intervalSeconds = 3f;

        public override ISentenceTriggerRuntime CreateRuntime(GameObject owner)
        {
            return new IntervalTriggerRuntime(owner, intervalSeconds);
        }
    }
}

