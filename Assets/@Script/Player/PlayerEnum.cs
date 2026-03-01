using System;
using UnityEngine;

public class PlayerEnum
{
    [Flags]
    public enum PlayerState
    {
        Idle = 1 << 0,
        Move = 1 << 1,
        Walk = 1 << 2,
        Run = 1 << 3,
        Crouch = 1 << 4,
        Flash = 1 << 5,
        Carry = 1 << 6,
        TimeStop = 1 << 7,
    }

    public enum Grounds
    {
        Untagged,
        Concrete,
        Wet,
        Shaft
    }

    public enum SkillIcon
    {
        AlwaysHide,
        AlwaysShow,
        HideWhenReady,
    }
}
