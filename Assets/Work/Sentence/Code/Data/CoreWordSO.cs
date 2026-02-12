using UnityEngine;
using Work.Sentence.Code.Runtime;
using Work.Sentence.Code.Runtime.Triggers;

namespace Work.Sentence.Code.Data
{
    [CreateAssetMenu(fileName = "CoreWord", menuName = "SO/Sentence/CoreWord", order = 0)]
    public class CoreWordSO : SentenceWordSO
    {
        [SerializeField] private SentenceTriggerSO trigger;
        public SentenceTriggerSO Trigger => trigger;
    }
}

