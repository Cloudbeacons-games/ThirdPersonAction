using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public EnemyAttackState(EnemyBrain enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        Debug.Log("Attack");
        animator.CrossFade(AttackHash,crossFadeDuration);
    }

    public override void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            agent.SetDestination(player.position);
        }
        enemy.Attack();
    }
}
