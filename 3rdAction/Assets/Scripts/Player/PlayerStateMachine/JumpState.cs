using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(JumpHash,crossFadeDuration);
        player.OnGroundContactLost();
        player.OnJumpStart();
    }

}
