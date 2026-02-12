using UnityEngine;

namespace Work.Sentence.Code.Data
{
    [CreateAssetMenu(fileName = "SentenceLoadout", menuName = "SO/Sentence/Loadout", order = 3)]
    public class SentenceLoadoutSO : ScriptableObject
    {
        [SerializeField] private SentencePartDefinitionSO[] parts;
        public SentencePartDefinitionSO[] Parts => parts;
    }
}

