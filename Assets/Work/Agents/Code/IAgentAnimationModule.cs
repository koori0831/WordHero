namespace Work.Agents.Code
{
    public interface IAgentAnimationModule : IAgentModule
    {
        void SetParam(int animHash, float value);
        void SetParam(int animHash, int value);
        void SetParam(int animHash, bool value);
        void SetApplyRootMotion(bool apply);
        float GetStateLength(int layer = 0);
    }
}
