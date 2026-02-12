using UnityEngine;

namespace Work.Sentence.Code.Data
{
    [CreateAssetMenu(fileName = "PortRuleSet", menuName = "SO/Sentence/PortRule/RuleSet", order = 5)]
    public class PortRuleSetSO : ScriptableObject
    {
        [SerializeField] private PortRuleSO[] rules;
        public PortRuleSO[] Rules => rules;
    }
}

