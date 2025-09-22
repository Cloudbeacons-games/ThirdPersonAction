using UnityEngine;

public class SlidingState : BaseState
{
    public SlidingState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(SlideHash, crossFadeDuration);
        player.OnGroundContactLost();
    }
}
