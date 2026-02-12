using System;

namespace Work.Sentence.Code.Runtime
{
    public enum BodyPart
    {
        Head = 0,
        Chest = 1,
        LeftArm = 2,
        RightArm = 3,
        Legs = 4,
        WeaponA = 5,
        WeaponB = 6,
    }

    [Flags]
    public enum PortType
    {
        None = 0,
        Attack = 1 << 0,
        Defense = 1 << 1,
        Utility = 1 << 2,
        Fire = 1 << 3,
        Ice = 1 << 4,
        Arcane = 1 << 5,
        Trigger = 1 << 6,
        Any = ~0,
    }

    [Flags]
    public enum WordCategory
    {
        None = 0,
        Trigger = 1 << 0,
        Modifier = 1 << 1,
        Effect = 1 << 2,
        Utility = 1 << 3,
        Any = ~0,
    }
}

