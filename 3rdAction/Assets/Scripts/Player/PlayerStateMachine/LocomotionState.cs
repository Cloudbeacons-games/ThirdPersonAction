using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(PlayerController player, Animator animator) : base(player, animator)
    {       
    }

    private bool cameraLocked = false;
    public override void OnEnter()
    {
        animator.CrossFade(LocomotionHash,crossFadeDuration);
        player.OnGroundContactRegained();
        player.canDash = true;
        cameraLocked = false;
    }

    public override void FixedUpdate()
    {
        if (player.cameraController.isCameraLocked && !cameraLocked)
        {
            cameraLocked = true;
            animator.CrossFade(LockedlocomotionHash, crossFadeDuration);
        }
        else if (!player.cameraController.isCameraLocked && cameraLocked)
        {
            cameraLocked = false;
            animator.CrossFade(LocomotionHash, crossFadeDuration);
        }
        if (player.GetMovementVelocity() == Vector3.zero)
        {
            animator.SetFloat("Horizontal", player.GetMovementVelocity().x, crossFadeDuration, Time.deltaTime);
            animator.SetFloat("Vertical", player.GetMovementVelocity().z, crossFadeDuration, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Horizontal", player.GetMovementVelocity().x);
            animator.SetFloat("Vertical", player.GetMovementVelocity().z);
        }
        
    }
}
