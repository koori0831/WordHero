using UnityEngine;
using Work.Sentence.Code.Runtime;
using Work.Sentence.Code.Runtime.Ports;

namespace Work.Sentence.Code.Data
{
    [CreateAssetMenu(fileName = "SourceCategoryRule", menuName = "SO/Sentence/PortRule/SpecificSourceCategoryRule", order = 4)]
    public class SpecificSourceCategoryRuleSO : PortRuleSO
    {
        [SerializeField] private string sourceWordId;
        [SerializeField] private WordCategory allowedTargetCategory = WordCategory.Any;
        [SerializeField] private bool allowWhenMatched = true;
        [SerializeField] private string reason = "Blocked by source-category rule.";

        public override bool TryEvaluate(in PortCompatibilityRequest request, out PortCompatibilityResult result)
        {
            if (request.From == null || request.To == null || string.IsNullOrEmpty(sourceWordId))
            {
                result = default;
                return false;
            }

            if (request.From.WordId != sourceWordId)
            {
                result = default;
                return false;
            }

            bool categoryMatched = (allowedTargetCategory & request.To.Category) != WordCategory.None;
            bool allowed = allowWhenMatched ? categoryMatched : !categoryMatched;
            result = allowed ? PortCompatibilityResult.Allowed(reason) : PortCompatibilityResult.Blocked(reason);
            return true;
        }
    }
}

