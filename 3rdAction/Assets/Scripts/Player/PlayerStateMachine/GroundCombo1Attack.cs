using UnityEngine;

public class GroundCombo1Attack : BaseAttackState
{
    public GroundCombo1Attack(PlayerController player, Animator animator, SwordScript swordScript) : base(player, animator,swordScript)
    {
    }

    public override void OnEnter()
    {
        player.isAttacking = true;
        isCombo = false;
        animator.CrossFade(GroundAttack2Hash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(player.attackKeyIsPressed);
        if (info.normalizedTime >= attackComboTimer && info.IsName("Melee Attack2") && player.attackKeyWasPressed)
        {
            isCombo = false;
        }
        if (info.normalizedTime >= 1f && info.IsName("Melee Attack2"))
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
