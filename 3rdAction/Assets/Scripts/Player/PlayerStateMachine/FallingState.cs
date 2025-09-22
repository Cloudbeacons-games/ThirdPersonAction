using UnityEngine;

public class FallingState : BaseState
{
    public FallingState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {

        animator.CrossFade(FallHash, crossFadeDuration);
        player.OnFallStart();
    }

}
