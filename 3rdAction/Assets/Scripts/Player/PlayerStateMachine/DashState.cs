using UnityEngine;

public class DashState : BaseState
{
    public DashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }
    public bool canAirDash = false;
    public override void OnEnter()
    {
        animator.CrossFade(DashHash, crossFadeDuration);
        player.OnDashStart();
        player.canDash = true;
    }

    public override void FixedUpdate()
    {
        if (player.stateMachine.PreviousState is not LocomotionState or SlidingState)
            player.canDash = canAirDash;
    }

    public override void OnExit()
    {
        player.OnDashEnd();
    }
}
