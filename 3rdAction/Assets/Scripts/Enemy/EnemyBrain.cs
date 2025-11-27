using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class EnemyBrain : MonoBehaviour
{
    [SerializeField]NavMeshAgent agent;
    [SerializeField]Animator animator;
    [SerializeField]PlayerDetector playerDetector;
    Rigidbody rb;


    StateMachine stateMachine;

    EnemyLifeManager lifeManager;

    CountdownTimer attackTimer;
    [SerializeField] float timeBetweenAttacks = 1f;
    EnemyBaseWeapon weapon;

    [SerializeField] private bool hitByAttack = false;
    

    public float wanderRadius;
    void Start()
    {
        EnemyManager.RegisterEnemy(this.gameObject);
        lifeManager = GetComponent<EnemyLifeManager>();
        attackTimer = new CountdownTimer(timeBetweenAttacks);
        weapon = GetComponent<EnemyBaseWeapon>();
        rb = GetComponent<Rigidbody>();

        stateMachine = new StateMachine();

        EnemyWanderState wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        EnemyChaseState chaseState = new EnemyChaseState(this, animator, agent, playerDetector.player);
        EnemyAttackState attackState = new EnemyAttackState(this, animator, agent, playerDetector.player, weapon);
        EnemyHitState hitState = new EnemyHitState(this, animator);

        At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()&&!playerDetector.isInRadius()));
        At(chaseState,attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => !attackTimer.IsRunning&&!playerDetector.CanAttackPlayer()));
        Any(hitState, new FuncPredicate(() => hitByAttack));
        At(hitState,chaseState,new FuncPredicate(()=>!hitByAttack&&isAwareOfEnemy()));
        At(hitState,wanderState,new FuncPredicate(()=>!hitByAttack&&!isAwareOfEnemy()));
        

        stateMachine.SetState(wanderState);

    }

    private bool isAwareOfEnemy()
    {
        return stateMachine.PreviousState is EnemyChaseState or EnemyAttackState;
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        if(agent.enabled)
            stateMachine.Update();
        else if(rb.linearVelocity.y < 0)
        {
            ResumeAgent();
        }
            
        attackTimer.Tick();

        
    }

    private void FixedUpdate()
    {
        if (agent.enabled)
            stateMachine.FixedUpdate();
    }

    public void Attack()
    {
        if(attackTimer.IsRunning)
            return;
        attackTimer.Start();
        Debug.Log("Attack");
    }

    public void TakeHit(float dmg)
    {
        lifeManager.TakeDamage(dmg);
        hitByAttack = true;
    }

    public void StopHitStun()
    {
        if(agent.enabled)
            hitByAttack = false;
    }

    public void KnockUp()
    {
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up*4,ForceMode.Impulse);

        
        
    }

    private void ResumeAgent()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        agent.enabled = true;
        
    }


}
