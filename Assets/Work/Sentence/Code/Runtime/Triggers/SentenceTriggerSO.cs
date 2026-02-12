using UnityEngine;

namespace Work.Sentence.Code.Runtime.Triggers
{
    public abstract class SentenceTriggerSO : ScriptableObject
    {
        public abstract ISentenceTriggerRuntime CreateRuntime(GameObject owner);
    }
}

