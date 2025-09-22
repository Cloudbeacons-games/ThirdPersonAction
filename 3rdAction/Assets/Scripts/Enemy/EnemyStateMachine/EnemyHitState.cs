using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
    public EnemyHitState(EnemyBrain enemy, Animator animator) : base(enemy, animator)
    {
        
    }

    public override void OnEnter()
    {
        animator.CrossFade(HitHash, crossFadeDuration);
    }

    public override void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=1&& animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            enemy.StopHitStun();
        }
    }
}
