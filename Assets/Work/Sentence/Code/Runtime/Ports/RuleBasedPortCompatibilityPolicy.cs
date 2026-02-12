using Work.Sentence.Code.Data;

namespace Work.Sentence.Code.Runtime.Ports
{
    public sealed class RuleBasedPortCompatibilityPolicy : IPortCompatibilityPolicy
    {
        private readonly IPortCompatibilityPolicy _defaultPolicy;
        private readonly PortRuleSO[] _rules;

        public RuleBasedPortCompatibilityPolicy(IPortCompatibilityPolicy defaultPolicy, PortRuleSO[] rules)
        {
            _defaultPolicy = defaultPolicy;
            _rules = rules;
        }

        public PortCompatibilityResult Evaluate(in PortCompatibilityRequest request)
        {
            if (_rules != null)
            {
                for (int i = 0; i < _rules.Length; i++)
                {
                    PortRuleSO rule = _rules[i];
                    if (rule == null) continue;

                    if (rule.TryEvaluate(in request, out PortCompatibilityResult overrideResult))
                    {
                        return overrideResult;
                    }
                }
            }

            return _defaultPolicy.Evaluate(in request);
        }
    }
}

