using UnityEngine;

public abstract class BaseState : IState
{
    protected readonly PlayerController player;
    protected readonly Animator animator;

    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int LockedlocomotionHash = Animator.StringToHash("Locked Locomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int DashHash = Animator.StringToHash("Dashing");
    protected static readonly int FallHash = Animator.StringToHash("Falling");
    protected static readonly int SlideHash = Animator.StringToHash("Sliding");
    protected static readonly int HitHash = Animator.StringToHash("Hit");
    protected static readonly int GroundAttack1Hash = Animator.StringToHash("Melee Attack1");
    protected static readonly int GroundAttack2Hash = Animator.StringToHash("Melee Attack2");

    protected const float crossFadeDuration = 0.1f;

    protected BaseState(PlayerController player,Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void OnEnter()
    {

    }
    public virtual void OnExit()
    {

    }
    public virtual void Update()
    {

    }
}
