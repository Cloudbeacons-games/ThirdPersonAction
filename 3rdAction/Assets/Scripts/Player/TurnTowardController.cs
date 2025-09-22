
using UnityEditor.Rendering;
using UnityEngine;

public class TurnTowardController : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] CameraController cameraController;
    public float turnSpeed = 50f;
    float angleDiffernce;
    float step;

    Transform tr;
    float currentYRotation;
    const float fallOffAngle = 90;

    private void Start()
    {
        tr  = transform;
        currentYRotation = tr.localEulerAngles.y;
    }

    private void LateUpdate()
    {
        if (controller.stateMachine.CurrentState is SlidingState)
        {
            currentYRotation = controller.GetMomentum().z>0?0f:180f;
            tr.localRotation = Quaternion.Euler(0, currentYRotation, 0);
        }



        Vector3 velocity = Vector3.ProjectOnPlane(controller.GetMovementVelocity(), tr.parent.up);
        if (velocity.magnitude < 0.001f||controller.stateMachine.CurrentState is BaseAttackState)
            return;

        if (cameraController.isCameraLocked)
        {
            tr.localRotation = Quaternion.Euler(0, cameraController.transform.localRotation.eulerAngles.y, 0);
            return;
        }

        angleDiffernce = VectorMath.GetAngle(tr.forward, velocity.normalized, tr.parent.up);
        step = Mathf.Sign(angleDiffernce) * Mathf.InverseLerp(0f, fallOffAngle, Mathf.Abs(angleDiffernce)) * Time.deltaTime * turnSpeed;



        currentYRotation += Mathf.Abs(step) > Mathf.Abs(angleDiffernce) ? angleDiffernce : step;
        tr.localRotation = Quaternion.Euler(0, currentYRotation, 0);
    }
}
