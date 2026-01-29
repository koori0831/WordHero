using Work.StatSystem.Code;

namespace Work.Sentence.Code
{
    public struct StatEffectSpec
    {
        public StatSO Stat;
        public StatModifierSpec Modifier;
        public object Key;

        public bool IsValid => Stat != null;
    }
}
