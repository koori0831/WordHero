using Work.Sentence.Code.Data;

namespace Work.Sentence.Code.Runtime.Ports
{
    public readonly struct PortCompatibilityRequest
    {
        public readonly SentenceWordSO From;
        public readonly SentenceWordSO To;

        public PortCompatibilityRequest(SentenceWordSO from, SentenceWordSO to)
        {
            From = from;
            To = to;
        }
    }

    public readonly struct PortCompatibilityResult
    {
        public readonly bool CanConnect;
        public readonly string Reason;

        public PortCompatibilityResult(bool canConnect, string reason)
        {
            CanConnect = canConnect;
            Reason = reason;
        }

        public static PortCompatibilityResult Allowed(string reason = null) => new PortCompatibilityResult(true, reason);
        public static PortCompatibilityResult Blocked(string reason) => new PortCompatibilityResult(false, reason);
    }
}

