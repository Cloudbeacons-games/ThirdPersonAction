using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    protected readonly EnemyBaseWeapon weapon;
    public EnemyAttackState(EnemyBrain enemy, Animator animator, NavMeshAgent agent, Transform player, EnemyBaseWeapon weapon) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
        this.weapon = weapon;
    }

    public override void OnEnter()
    {
        Debug.Log("Attack");
        animator.CrossFade(AttackHash,0f);
    }

    public override void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            agent.SetDestination(player.position);
        }
        enemy.Attack();
    }

    public override void OnExit()
    {
        animator.StopPlayback();
        weapon.DisableCollider();
    }
}
