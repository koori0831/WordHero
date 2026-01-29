namespace Work.Sentence.Code
{
    public sealed class SkillInstance
    {
        public SkillKind Kind;
        public string DebugName;

        public float Cooldown;
        public float Damage;

        public float Duration;
        public float Magnitude; // buff/cc 강도

        public TagSet Tags;

        public bool HasStatEffect;
        public StatEffectSpec StatEffect;
    }
}
