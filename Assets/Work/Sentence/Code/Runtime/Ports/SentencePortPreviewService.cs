using Work.Sentence.Code.Data;

namespace Work.Sentence.Code.Runtime.Ports
{
    public sealed class SentencePortPreviewService
    {
        private readonly IPortCompatibilityPolicy _policy;

        public SentencePortPreviewService(IPortCompatibilityPolicy policy)
        {
            _policy = policy;
        }

        public PortCompatibilityResult Preview(SentenceWordSO from, SentenceWordSO to)
        {
            PortCompatibilityRequest request = new PortCompatibilityRequest(from, to);
            return _policy.Evaluate(in request);
        }
    }
}

