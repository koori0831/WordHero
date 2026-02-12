using UnityEngine;
using Work.Sentence.Code.Runtime;

namespace Work.Sentence.Code.Data
{
    [CreateAssetMenu(fileName = "SentencePart", menuName = "SO/Sentence/PartDefinition", order = 2)]
    public class SentencePartDefinitionSO : ScriptableObject
    {
        [SerializeField] private BodyPart bodyPart;
        [SerializeField] private CoreWordSO coreWord;
        [SerializeField] private ModifierWordSO[] modifierWords;

        public BodyPart BodyPart => bodyPart;
        public CoreWordSO CoreWord => coreWord;
        public ModifierWordSO[] ModifierWords => modifierWords;
    }
}

