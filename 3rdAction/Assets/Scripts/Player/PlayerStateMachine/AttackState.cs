using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        player.isAttacking = true;
        animator.CrossFade(GroundAttack1Hash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Melee Attack1"))
            player.isAttacking = false;
    }
}
