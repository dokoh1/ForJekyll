using MBT;
using NPC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.AI;

[AddComponentMenu("")]
[MBTNode("Services/Check Player State Service")]
public class CheckPlayerStateService : Service
{
    public IntReference curState;
    public BoolReference isCrouch;

    public Player player;
    public Animator animator;
    public CapsuleCollider capsuleCollider;
    public NpcUnit npcUnit;

    private PlayerEnum.PlayerState _playerCrouchState = PlayerEnum.PlayerState.Crouch;

    public override void Task()
    {
        if (GameManager.Instance.IsTimeStop) return;

        if (player == null) player = GameManager.Instance.Player;

        if (player.CurState.HasFlag(_playerCrouchState))
        {
            SetCrouch();
        }
        else
        {
            if (npcUnit.IsCrouching)
            {
                SetCrouch();
                return;
            } 
            npcUnit.CurNoiseAmount = 7f;
            isCrouch.Value = false;
            curState.Value = (int)((NPCState)curState.Value & ~NPCState.Crouch);
            capsuleCollider.center = new Vector3(0f, 0.91f, 0f);
            capsuleCollider.height = 1.82f;
            animator.SetBool("@Crouch", false);
        }
    }

    private void SetCrouch()
    {
        npcUnit.CurNoiseAmount = 0f;
        isCrouch.Value = true;
        curState.Value = (int)((NPCState)curState.Value | NPCState.Crouch);
        capsuleCollider.center = new Vector3(0f, 0.66f, 0f);
        capsuleCollider.height = 1.32f;
        animator.SetBool("@Crouch", true);
    }
}
