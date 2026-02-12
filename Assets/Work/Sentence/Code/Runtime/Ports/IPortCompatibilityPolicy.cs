namespace Work.Sentence.Code.Runtime.Ports
{
    public interface IPortCompatibilityPolicy
    {
        PortCompatibilityResult Evaluate(in PortCompatibilityRequest request);
    }
}

