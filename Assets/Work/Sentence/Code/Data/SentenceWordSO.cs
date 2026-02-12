using UnityEngine;
using Work.Sentence.Code.Runtime;

namespace Work.Sentence.Code.Data
{
    public abstract class SentenceWordSO : ScriptableObject
    {
        [SerializeField] private string wordId;
        [SerializeField] private string displayName;
        [SerializeField] private int weight;
        [SerializeField] private WordCategory category = WordCategory.Modifier;
        [SerializeField] private PortType inputPorts = PortType.None;
        [SerializeField] private PortType outputPorts = PortType.None;
        [SerializeField] private WordCategory allowedTargetCategories = WordCategory.Any;

        public string WordId => wordId;
        public string DisplayName => displayName;
        public int Weight => weight;
        public WordCategory Category => category;
        public PortType InputPorts => inputPorts;
        public PortType OutputPorts => outputPorts;
        public WordCategory AllowedTargetCategories => allowedTargetCategories;
    }
}

