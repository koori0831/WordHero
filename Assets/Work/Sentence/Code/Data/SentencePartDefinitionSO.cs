using UnityEngine;
using Work.Sentence.Code.Runtime;
using System.Collections.Generic;

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

        public void SetModifierWordsFromEquipped(IReadOnlyList<ModifierWordSO> equippedWords, int slotCount = 2)
        {
            if (slotCount < 0)
            {
                slotCount = 0;
            }

            if (modifierWords == null || modifierWords.Length != slotCount)
            {
                modifierWords = new ModifierWordSO[slotCount];
            }

            for (int i = 0; i < modifierWords.Length; i++)
            {
                modifierWords[i] = null;
            }

            if (equippedWords == null || equippedWords.Count == 0)
            {
                return;
            }

            int count = Mathf.Min(modifierWords.Length, equippedWords.Count);
            for (int i = 0; i < count; i++)
            {
                modifierWords[i] = equippedWords[i];
            }
        }
    }
}

