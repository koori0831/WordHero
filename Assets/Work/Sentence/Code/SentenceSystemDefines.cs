using System;

namespace Work.Sentence.Code
{
    public enum PartOfSpeech
    {
        Subject,
        Object,
        Verb,
        Option
    }
    public enum DirectActionType
    {
        None,
        BasicAttack,
        Dodge
    }
    public enum NumericKind
    {
        None,
        DurationSeconds,
        Percent,
        Flat
    }

    public struct NumericToken
    {
        public bool HasValue;
        public NumericKind Kind;
        public float Value;

        public static NumericToken None => new() { HasValue = false, Kind = NumericKind.None, Value = 0f };
    }

    [Flags]
    public enum TargetTag { None = 0, Self = 1 << 0, Enemy = 1 << 1, Area = 1 << 2 }

    [Flags]
    public enum FormTag { None = 0, Melee = 1 << 0, Projectile = 1 << 1, AoE = 1 << 2 }

    [Flags]
    public enum ElementTag { None = 0, Fire = 1 << 0, Ice = 1 << 1, Poison = 1 << 2 }

    [Flags]
    public enum ModifierTag { None = 0, PowerUp = 1 << 0, RangeUp = 1 << 1, CastTimeDown = 1 << 2 }

    [Serializable]
    public struct TagSet
    {
        public TargetTag Target;
        public FormTag Form;
        public ElementTag Element;
        public ModifierTag Modifier;

        public static TagSet Merge(TagSet a, TagSet b) => new()
        {
            Target = a.Target | b.Target,
            Form = a.Form | b.Form,
            Element = a.Element | b.Element,
            Modifier = a.Modifier | b.Modifier
        };
    }

    [Serializable]
    public struct TagQuery
    {
        public TargetTag RequireTargetAny;
        public FormTag RequireFormAny;
        public ElementTag RequireElementAny;
        public ModifierTag RequireModifierAny;

        public bool Matches(TagSet tags)
        {
            if (RequireTargetAny != 0 && (tags.Target & RequireTargetAny) == 0) return false;
            if (RequireFormAny != 0 && (tags.Form & RequireFormAny) == 0) return false;
            if (RequireElementAny != 0 && (tags.Element & RequireElementAny) == 0) return false;
            if (RequireModifierAny != 0 && (tags.Modifier & RequireModifierAny) == 0) return false;
            return true;
        }

        public int Score(TagSet tags)
        {
            int s = 0;
            if (RequireTargetAny != 0 && (tags.Target & RequireTargetAny) != 0) s += 10;
            if (RequireFormAny != 0 && (tags.Form & RequireFormAny) != 0) s += 10;
            if (RequireElementAny != 0 && (tags.Element & RequireElementAny) != 0) s += 10;
            if (RequireModifierAny != 0 && (tags.Modifier & RequireModifierAny) != 0) s += 5;
            return s;
        }
    }

    public enum SkillKind
    {
        Attack,
        Buff,
        CC,
        Special
    }
}