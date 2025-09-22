using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Vector3 startPos;
    readonly float wanderRadius;

    private float wanderSpeed = 4f;
    public EnemyWanderState(EnemyBrain enemy, Animator animator,NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
    {
        this.agent = agent;
        this.startPos = enemy.transform.position;
        this.wanderRadius = wanderRadius;
    }

    public override void OnEnter()
    {
        Debug.Log("Wander");
        agent.speed = wanderSpeed;
        animator.CrossFade(WalkHash, crossFadeDuration);

    }

    public override void Update()
    {
        if(HasReachedDestination())
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            Vector3 finalPosition = hit.position;

            agent.SetDestination(finalPosition);
        }
    }

    bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath||agent.velocity.sqrMagnitude==0f);
    }

    
}
