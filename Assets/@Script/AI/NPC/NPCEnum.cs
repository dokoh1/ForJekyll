using System;

namespace NPC
{
    [Flags]
    public enum NPCState
    {
        Wait = 1 << 0,
        Idle = 1 << 1,
        Move = 1 << 2,
        Run = 1 << 3,
        Crouch = 1 << 4,
        Dead = 1 << 5
    }

    [Flags]
    public enum NPCSetting
    {
        CanTalk = 1 << 0,
        CanFollow = 1 << 1,
        CanRepeat = 1 << 2,
        HaveDestination = 1 << 3,
        StartFollowing = 1 << 4
    }

    public enum NpcSoundKey
    {
        Scream = 1,

        
    }
}

