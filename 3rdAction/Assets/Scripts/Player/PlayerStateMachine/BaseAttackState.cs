using UnityEngine;

public class BaseAttackState : BaseState
{
    protected readonly SwordScript swordScript;

    protected float attackComboTimer = 0.5f;
    protected bool isCombo;
    public BaseAttackState(PlayerController player, Animator animator,SwordScript swordScript) : base(player, animator)
    {
        this.swordScript = swordScript;
    }

    public override void OnExit()
    {
        animator.StopPlayback();
        swordScript.DisableCollider();
    }
}
