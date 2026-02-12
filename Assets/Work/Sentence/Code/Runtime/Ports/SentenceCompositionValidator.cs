using System.Collections.Generic;
using Work.Sentence.Code.Data;

namespace Work.Sentence.Code.Runtime.Ports
{
    public sealed class SentenceCompositionValidator
    {
        private readonly IPortCompatibilityPolicy _policy;

        public SentenceCompositionValidator(IPortCompatibilityPolicy policy)
        {
            _policy = policy;
        }

        public bool Validate(CoreWordSO core, ModifierWordSO[] modifiers, List<PortCompatibilityResult> issues)
        {
            if (core == null) return false;

            SentenceWordSO previous = core;
            if (modifiers == null || modifiers.Length == 0) return true;

            for (int i = 0; i < modifiers.Length; i++)
            {
                ModifierWordSO current = modifiers[i];
                if (current == null) continue;

                PortCompatibilityRequest request = new PortCompatibilityRequest(previous, current);
                PortCompatibilityResult result = _policy.Evaluate(in request);
                if (!result.CanConnect)
                {
                    issues?.Add(result);
                    return false;
                }

                previous = current;
            }

            return true;
        }
    }
}

