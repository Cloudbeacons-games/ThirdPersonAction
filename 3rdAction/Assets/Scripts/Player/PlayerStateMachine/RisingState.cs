using UnityEngine;

public class RisingState : BaseState
{
    public RisingState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {

        animator.CrossFade(JumpHash, crossFadeDuration);
        player.OnGroundContactLost();
    }
}
