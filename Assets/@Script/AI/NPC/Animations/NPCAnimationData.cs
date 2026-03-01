using System;
using UnityEngine;
[Serializable]
public class NPCAnimationData
{
    [SerializeField] private string followParameterName = "@Follow";
    [SerializeField] private string idleParameterName = "Idle";

    public int FollowParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }

    public void Initialize()
    {
        FollowParameterHash = Animator.StringToHash(followParameterName);
        IdleParameterHash = Animator.StringToHash(idleParameterName);
     
    }
}
