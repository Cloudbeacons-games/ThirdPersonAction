using UnityEngine;

public class GroundAttackStart : BaseAttackState
{
    public GroundAttackStart(PlayerController player, Animator animator,SwordScript swordScript) : base(player, animator,swordScript)
    {
    }

    public override void OnEnter()
    {
        player.isAttacking = true;
        isCombo = false;
        animator.CrossFade(GroundAttack1Hash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        
        if (info.normalizedTime >= attackComboTimer && info.IsName("Melee Attack1") && player.attackKeyWasPressed)
        {
            isCombo = true;            
        }
        if (info.normalizedTime >= 1f && info.IsName("Melee Attack1"))
            if (!isCombo)
            {
                player.isAttacking = false;
            }
            else
            {
                player.isCombo = true;
            }
        
    }

    public override void OnExit()
    {
        base.OnExit();
        player.isCombo = false;
    }
}
