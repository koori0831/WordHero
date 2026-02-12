using Work.Sentence.Code.Runtime;

namespace Work.Sentence.Code.Runtime.Ports
{
    public sealed class DefaultPortCompatibilityPolicy : IPortCompatibilityPolicy
    {
        public PortCompatibilityResult Evaluate(in PortCompatibilityRequest request)
        {
            if (request.From == null || request.To == null)
            {
                return PortCompatibilityResult.Blocked("Word reference is null.");
            }

            PortType overlap = request.From.OutputPorts & request.To.InputPorts;
            if (overlap == PortType.None)
            {
                return PortCompatibilityResult.Blocked("Output/Input port mismatch.");
            }

            if ((request.From.AllowedTargetCategories & request.To.Category) == WordCategory.None)
            {
                return PortCompatibilityResult.Blocked("Target category is not allowed.");
            }

            return PortCompatibilityResult.Allowed();
        }
    }
}

