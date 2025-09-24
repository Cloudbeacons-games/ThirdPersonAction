using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMover))]
public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] InputReader input;
    Transform playerTransform;
    PlayerMover playerMover;
    public CameraController cameraController;
    public SwordScript swordScript;
    public PlayerLifeManager playerLifeManager;

    bool jumpInputIsLocked, jumpKeyWasPressed, jumpKeyWasLetGo, JumpKeyIsPressed;
    public bool attackKeyWasPressed, attackKeyIsPressed;
    public bool canDash=true;
    bool dashInputIsLocked,dashKeyWasPressed, dashKeyIsPressed;

    public float movementSpeed=7f;
    public float airControlRate = 2f;
    public float jumpSpeed = 10f;
    public float jumpDuration = 0.2f;
    public float airFriction = 0.5f;
    public float groundFriction = 100f;
    public float gravity = 30f;
    public float slideGravity = 5f;
    public float slopeLimit = 50f;
    public bool useLocalMomentum;
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.3f;

    public bool isAttacking = false;
    public bool isCombo = false;
    private bool isBeingHit = false;

    public StateMachine stateMachine;
    CeilingDetector ceilingDetector;
    CountdownTimer jumpTimer,dashTimer,dashCooldownTimer;

    [SerializeField] Transform cameraTransform;
    [SerializeField] Animator animator;

    Vector3 momentum, savedVelocity, savedMomentumVelocity;

    public event Action<Vector3> OnJump = delegate { };
    public event Action<Vector3> OnLand = delegate { };


    #endregion

    private void Awake()
    {
        playerTransform = transform;
        playerMover = GetComponent<PlayerMover>();
        input.EnablePlayerActions();


        jumpTimer = new CountdownTimer(jumpDuration);
        dashTimer = new CountdownTimer(dashDuration);
        dashCooldownTimer = new CountdownTimer(dashCooldown);
        ceilingDetector = GetComponent<CeilingDetector>();

        //get components
        SetupStateMachine();
    }
    private void Start()
    {
        
        input.Jump += HandleJumpKeyInput;
        input.Dash += HandleDashKeyInput;
        input.Attack += HandleAttackKeyInput;
    }

    private void HandleAttackKeyInput(bool isButtonPressed)
    {
        if (!attackKeyIsPressed && isButtonPressed)
        {
            attackKeyWasPressed = true;
        }

        attackKeyIsPressed = isButtonPressed;
        
    }

    private void HandleJumpKeyInput(bool isButtonPressed)
    {
        if(!JumpKeyIsPressed&&isButtonPressed)
        {
            jumpKeyWasPressed = true;
        }

        if(JumpKeyIsPressed && !isButtonPressed)
        {
            jumpKeyWasLetGo = true;
            jumpInputIsLocked = false;
        }

        JumpKeyIsPressed = isButtonPressed;
    }

    private void HandleDashKeyInput(bool isButtonPressed)
    {
        if (!dashKeyIsPressed && isButtonPressed && !dashCooldownTimer.IsRunning)
        {
            dashKeyWasPressed = true;
        }

        if (dashKeyIsPressed && !isButtonPressed)
        {
             
            dashInputIsLocked = false;
        }

        if(!dashCooldownTimer.IsRunning)
            dashKeyIsPressed = isButtonPressed;
    }

    private void SetupStateMachine()
    {
        //state machine
        stateMachine = new StateMachine();
        //states Declaration
        LocomotionState locomotionState = new LocomotionState(this, animator);
        JumpState jumpState = new JumpState(this, animator);
        SlidingState slidsingState = new SlidingState(this, animator);
        RisingState risingState = new RisingState(this, animator);
        FallingState fallingState = new FallingState(this, animator);
        DashState dashState = new DashState(this, animator);
        GroundAttackStart groundAttackState = new GroundAttackStart(this, animator,swordScript);
        GroundCombo1Attack groundCombo1Attack = new GroundCombo1Attack(this,animator,swordScript);
        HitState hitState = new HitState(this, animator);
        //define Transitions
        At(locomotionState, risingState, new FuncPredicate(()=> IsRising()));
        At(locomotionState, slidsingState, new FuncPredicate(()=> playerMover.IsGrounded()&&IsGroundTooSteep()));
        At(locomotionState, fallingState, new FuncPredicate(() => !playerMover.IsGrounded()));
        At(locomotionState, jumpState, new FuncPredicate(() =>(JumpKeyIsPressed||jumpKeyWasPressed)&&!jumpInputIsLocked));
        At(locomotionState, groundAttackState, new FuncPredicate(() =>(attackKeyIsPressed||attackKeyWasPressed)&&input.WasAttackPressedThisFrame()));

        At(fallingState,risingState, new FuncPredicate(()=>IsRising()));
        At(fallingState,locomotionState, new FuncPredicate(()=>playerMover.IsGrounded()&&!IsGroundTooSteep()));
        At(fallingState,slidsingState, new FuncPredicate(()=>playerMover.IsGrounded()&&IsGroundTooSteep()));

        At(slidsingState, risingState, new FuncPredicate(() => IsRising()));
        At(slidsingState, fallingState, new FuncPredicate(() => !playerMover.IsGrounded()));
        At(slidsingState, locomotionState, new FuncPredicate(() => playerMover.IsGrounded()&&!IsGroundTooSteep()));

        At(risingState, locomotionState, new FuncPredicate(() => playerMover.IsGrounded() && !IsGroundTooSteep()));
        At(risingState, slidsingState, new FuncPredicate(() => playerMover.IsGrounded() && IsGroundTooSteep()));
        At(risingState, fallingState, new FuncPredicate(() => IsFalling()));
        At(risingState, fallingState, new FuncPredicate(() => ceilingDetector != null && ceilingDetector.HitCeiling()));

        At(jumpState, risingState, new FuncPredicate(() => jumpTimer.IsFinished || jumpKeyWasLetGo));
        At(jumpState, fallingState, new FuncPredicate(() => ceilingDetector != null && ceilingDetector.HitCeiling()));

        At(dashState, locomotionState, new FuncPredicate(() => playerMover.IsGrounded() && !IsGroundTooSteep() && dashTimer.IsFinished));
        At(dashState, slidsingState, new FuncPredicate(() => playerMover.IsGrounded() && IsGroundTooSteep() && dashTimer.IsFinished));
        At(dashState, fallingState, new FuncPredicate(() => !playerMover.IsGrounded() && dashTimer.IsFinished));

        At(groundAttackState, locomotionState, new FuncPredicate(() =>!isCombo && !isAttacking && playerMover.IsGrounded() && !IsGroundTooSteep()));
        At(groundAttackState, slidsingState, new FuncPredicate(() => !isCombo && !isAttacking && playerMover.IsGrounded() && IsGroundTooSteep()));
        At(groundAttackState, fallingState, new FuncPredicate(() => !isCombo && !isAttacking && !playerMover.IsGrounded()));
        At(groundAttackState, groundCombo1Attack, new FuncPredicate(() => isCombo));

        At(groundCombo1Attack, locomotionState, new FuncPredicate(() => !isAttacking && playerMover.IsGrounded() && !IsGroundTooSteep()));
        At(groundCombo1Attack, slidsingState, new FuncPredicate(() => !isAttacking && playerMover.IsGrounded() && IsGroundTooSteep()));
        At(groundCombo1Attack, fallingState, new FuncPredicate(() => !isAttacking && !playerMover.IsGrounded()));

        At(hitState, locomotionState, new FuncPredicate(() =>!isBeingHit && playerMover.IsGrounded() && !IsGroundTooSteep()));
        At(hitState, fallingState, new FuncPredicate(() =>!isBeingHit && !playerMover.IsGrounded()));
        At(hitState, slidsingState, new FuncPredicate(() =>!isBeingHit && playerMover.IsGrounded() && IsGroundTooSteep()));



        Any(hitState, new FuncPredicate(() =>isBeingHit));
        Any(dashState, new FuncPredicate(()=> (dashKeyIsPressed || dashKeyWasPressed) && !dashInputIsLocked && !dashCooldownTimer.IsRunning && canDash));
       
        //set initial state
        stateMachine.SetState(fallingState);
    }


    void At(IState from, IState to,IPredicate condition)=>stateMachine.AddTransition(from, to, condition);
    void Any(IState to , IPredicate condition)=>stateMachine.AddAnyTransition(to, condition);

    bool IsRising() => VectorMath.GetDotProduct(GetMomentum(), playerTransform.up) > 0f;
    bool IsFalling() => VectorMath.GetDotProduct(GetMomentum(), playerTransform.up) < 0f;
    bool IsGroundTooSteep() => Vector3.Angle(playerMover.GetGroundNormal(),playerTransform.up) >slopeLimit || playerMover.GetGroundLayer() == LayerMask.NameToLayer("Slope Layer")|| !playerMover.IsGrounded();


    public Vector3 GetMomentum() => useLocalMomentum ? playerTransform.localToWorldMatrix*momentum : momentum;


    private void Update()
    {
        //Debug.Log();
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        playerMover.CheckForGround();
        
        HandleMomentum();
        Vector3 velocity = stateMachine.CurrentState is LocomotionState ? CalculateMovementVelocity():Vector3.zero;
        if (stateMachine.CurrentState is not BaseAttackState)
            velocity += useLocalMomentum ? playerTransform.localToWorldMatrix * momentum : momentum;
        else
            momentum = Vector3.zero;

        playerMover.SetExtendedSensorRange(IsGrounded());


        playerMover.SetVelocity(velocity);
        savedVelocity = velocity;
        savedMomentumVelocity = CalculateMovementVelocity();

        ResetJumpKeys();
        ResetDashKeys();
        ResetAttackKeys();
        if(ceilingDetector!=null)
            ceilingDetector.Reset();
    }


    private void HandleMomentum()
    {
        if(useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix*momentum;

        Vector3 verticalMomentum = VectorMath.ExtractDotVector(momentum, playerTransform.up);
        Vector3 horizontalMomentum = momentum - verticalMomentum;

        verticalMomentum -= playerTransform.up * (gravity * Time.deltaTime);

        if(stateMachine.CurrentState is LocomotionState or BaseAttackState or HitState&&VectorMath.GetDotProduct(verticalMomentum,playerTransform.up)<0f)
        {
            verticalMomentum = Vector3.zero;
        }

        if(!IsGrounded())
        {
            AdjustHorizontalMomentum(ref horizontalMomentum, CalculateMovementVelocity());
        }
        if (stateMachine.CurrentState is SlidingState)
        {
            HandleSliding(ref horizontalMomentum);
        }
        if(stateMachine.CurrentState is DashState)
        {
            HandleDashing(ref horizontalMomentum);
            
        }

        //fricition
        
        float friction = stateMachine.CurrentState is LocomotionState ? groundFriction : airFriction;
        horizontalMomentum = Vector3.MoveTowards(horizontalMomentum,Vector3.zero,friction*Time.deltaTime);

        //no friction
        //horizontalMomentum = Vector3.MoveTowards(horizontalMomentum,Vector3.zero,1f);

        momentum = horizontalMomentum + verticalMomentum;
        


        //handleJump
        if (stateMachine.CurrentState is JumpState)
        {
            HandleJumping();
        }
        if(stateMachine.CurrentState is SlidingState)
        {
            momentum = Vector3.ProjectOnPlane(momentum, playerMover.GetGroundNormal());
            if (VectorMath.GetDotProduct(momentum, playerTransform.up) > 0f)
                momentum = VectorMath.RemoveDotVector(momentum, playerTransform.up);

            Vector3 slideDirection = Vector3.ProjectOnPlane(-playerTransform.up, playerMover.GetGroundNormal()).normalized;
            momentum += slideDirection * (slideGravity * Time.deltaTime);
        }

        if(stateMachine.CurrentState is DashState)
        {
            //momentum = Vector3.ProjectOnPlane(momentum, playerMover.GetGroundNormal());
            Vector3 dashingDown = Vector3.ProjectOnPlane(momentum, playerMover.GetGroundNormal());

            if (VectorMath.GetDotProduct(dashingDown, playerTransform.up) < 0f)
                momentum = Vector3.ProjectOnPlane(momentum, playerMover.GetGroundNormal());
            else
            {
                Vector3 upDir= VectorMath.ExtractDotVector(momentum, playerTransform.up);
                momentum = VectorMath.RemoveDotVector(momentum, playerTransform.up);
                momentum += -upDir;
            }
           
        }
           

        if (useLocalMomentum)
            momentum=playerTransform.localToWorldMatrix*momentum;
    }

 

    void ResetJumpKeys()
    {
        jumpKeyWasLetGo = false;
        jumpKeyWasPressed = false;
    }

    void ResetDashKeys()
    {
        dashKeyWasPressed = false;
    }
    void ResetAttackKeys()
    {
        attackKeyWasPressed = false;
    }

    public void GotHit(float dmg)
    {
        isBeingHit = true;
        playerLifeManager.TakeDamage(dmg);
    }

    public void StopHit()
    {
        isBeingHit = false;
    }


    public void OnJumpStart()
    {
        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;

        momentum += playerTransform.up * jumpSpeed;
        jumpTimer.Start();
        jumpInputIsLocked = true;
        OnJump.Invoke(momentum);

        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;
    }
    public void OnDashStart()
    {
        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;
        //momentum += Vector3.ProjectOnPlane(cameraTransform.forward, playerTransform.up).normalized * dashSpeed;//dash where camera looks
        Vector3 dashVelocity = (Vector3.ProjectOnPlane(cameraTransform.right, playerTransform.up).normalized * input.Direction.x+
        Vector3.ProjectOnPlane(cameraTransform.forward, playerTransform.up).normalized * (input.Direction.y!=0f? input.Direction.y:1f)).normalized;//dash where input looks

        momentum += dashVelocity * dashSpeed;
       

        dashTimer.Start();
        dashCooldownTimer.Reset();
        dashInputIsLocked = true;

        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;
    }

    public void OnDashEnd()
    {
        dashCooldownTimer.Start();
        dashKeyIsPressed = false;
    }


    public void OnGroundContactLost()
    {
        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;

        Vector3 velocity = GetMovementVelocity();
        if(velocity.sqrMagnitude > 0 && momentum.sqrMagnitude > 0)
        {
            Vector3 projectedMomentum = Vector3.Project(momentum,velocity.normalized);
            float dot = VectorMath.GetDotProduct(projectedMomentum.normalized, velocity.normalized);

            if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f)
                velocity = Vector3.zero;
            else if (dot > 0f)
                velocity -= projectedMomentum;
        }
        momentum += velocity;
        if(useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;
    }

    public Vector3 GetMovementVelocity() => savedMomentumVelocity;

    public void OnGroundContactRegained()
    {
        Vector3 collisionVelocity = useLocalMomentum ? playerTransform.localToWorldMatrix*momentum : momentum;
        OnLand.Invoke(collisionVelocity);
    }

    public void OnFallStart()
    {
        var currentUpMomentum = VectorMath.ExtractDotVector(momentum, playerTransform.up);
        momentum = VectorMath.RemoveDotVector(momentum,playerTransform.up);
        momentum-= transform.up*currentUpMomentum.magnitude;
    }
    private void HandleJumping()
    {
        momentum = VectorMath.RemoveDotVector(momentum,playerTransform.up);
        momentum += playerTransform.up * jumpSpeed;
    }
    

    private void HandleSliding(ref Vector3 horizontalMomentum)
    {
        Vector3 pointDownVector = Vector3.ProjectOnPlane(playerMover.GetGroundNormal(), playerTransform.up).normalized;
        Vector3 movementVelocity = CalculateMovementVelocity();

        movementVelocity = VectorMath.RemoveDotVector(movementVelocity, pointDownVector);
        horizontalMomentum += movementVelocity * Time.fixedDeltaTime;

        
    }

    private void HandleDashing(ref Vector3 horizontalMomentum)
    {
        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;
        //momentum += Vector3.ProjectOnPlane(cameraTransform.forward, playerTransform.up).normalized * dashSpeed;//dash where camera looks
        horizontalMomentum += CalculateMovementVelocity() * Time.fixedDeltaTime;//dash where input looks


        if (useLocalMomentum)
            momentum = playerTransform.localToWorldMatrix * momentum;
    }

    void AdjustHorizontalMomentum(ref Vector3 horizontalMomentum,Vector3 movementVelocity)
    {
        if (horizontalMomentum.magnitude > movementSpeed)
        {
            if (VectorMath.GetDotProduct(movementVelocity, horizontalMomentum.normalized) > 0f)
            {
                movementVelocity = VectorMath.RemoveDotVector(movementVelocity, horizontalMomentum.normalized);
            }
            horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate * 0.25f);
        }
        else
        {
            horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate);
            horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, movementSpeed);
        }
    }

    private Vector3 CalculateMovementVelocity() => CalculateMovmentDirection() * movementSpeed;

    private Vector3 CalculateMovmentDirection()
    {
        Vector3 direction = cameraTransform == null
            ? playerTransform.right * input.Direction.x + playerTransform.forward * input.Direction.y
            : Vector3.ProjectOnPlane(cameraTransform.right, playerTransform.up).normalized * input.Direction.x +
            Vector3.ProjectOnPlane(cameraTransform.forward, playerTransform.up).normalized * input.Direction.y;


        return direction.magnitude > 1f ? direction.normalized : direction;
    }

    bool IsGrounded() => stateMachine.CurrentState is LocomotionState or SlidingState or BaseAttackState;
}
