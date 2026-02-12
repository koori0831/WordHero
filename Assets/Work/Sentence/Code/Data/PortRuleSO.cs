using UnityEngine;
using Work.Sentence.Code.Runtime.Ports;

namespace Work.Sentence.Code.Data
{
    public abstract class PortRuleSO : ScriptableObject
    {
        public abstract bool TryEvaluate(in PortCompatibilityRequest request, out PortCompatibilityResult result);
    }
}

