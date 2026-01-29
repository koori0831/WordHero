namespace Work.StatSystem.Code
{
    public enum StatModOp
    {
        Add, // +n
        Mul, // *(1 + n)
    }

    /// <summary>
    /// 한 종류의 modifier "스펙".
    /// key 기준으로 스택 쌓고, duration 있으면 시간제.
    /// </summary>
    public readonly record struct StatModifierSpec(
        StatModOp Op,
        float ValuePerStack,
        int MaxStacks = 1,
        float? DurationSeconds = null,
        bool RefreshDurationOnStack = true,
        int Priority = 0
    )
    {
        public bool IsTimed => DurationSeconds.HasValue && DurationSeconds.Value > 0f;

        public static StatModifierSpec Add(float value, int maxStacks = 1, float? duration = null, bool refresh = true, int priority = 0)
            => new(StatModOp.Add, value, maxStacks, duration, refresh, priority);

        public static StatModifierSpec Mul(float value, int maxStacks = 1, float? duration = null, bool refresh = true, int priority = 0)
            => new(StatModOp.Mul, value, maxStacks, duration, refresh, priority);
    }
}
