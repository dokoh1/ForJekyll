using System;

[Flags]
public enum MonsterState
{
    Idle = 1,
    Walk = 2,
    Run = 3,
    Attack = 4,
    Lost = 5,
    Find = 6,
}

[Flags]
public enum MonsterType
{
    EyeType = 1,
    EarType = 2
}

[Flags]
public enum MonsterSetting
{
    IsWork = 1 << 0,
    CanPatrol = 1 << 1,
    KeepPosition = 1 << 2,
    HaveDestination = 1 << 3,
    HaveTarget = 1 << 4,
}

public enum MonsterSoundKey
{
    Idle = 1,
    Chase = 2,
    Attack = 3,
    Lost = 4,
    Find = 5,
    Eat = 6,
}