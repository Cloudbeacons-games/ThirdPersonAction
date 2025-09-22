using UnityEngine;

public class HitState : BaseState
{
    public HitState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(HitHash,crossFadeDuration);
    }

    public override void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            player.StopHit();
        }
    }
}
