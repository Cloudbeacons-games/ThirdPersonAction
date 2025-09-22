using System;
using UnityEngine;

public class CameraDistanceRaycaster : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform cameraTargetTransform;

    public LayerMask layerMask = Physics.AllLayers;

    public float minimumDistanceFrmoObstacles = 0.1f;
    public float smoothingFactor = 25f;

    Transform tr;
    float currentDistance;

    private void Awake()
    {
        tr = transform;
        layerMask &= ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        currentDistance = (cameraTargetTransform.position - tr.position).magnitude;
    }

    private void LateUpdate()
    {
        Vector3 castDirection = cameraTargetTransform.position - tr.position;

        float distance = GetCameraDistance(castDirection);

        currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * smoothingFactor);
        cameraTransform.position = tr.position + castDirection.normalized * currentDistance;
    }

    private float GetCameraDistance(Vector3 castDirection)
    {
        float distance = castDirection.magnitude + minimumDistanceFrmoObstacles;
        float sphereRadius = 0.5f;
        if(Physics.SphereCast(new Ray(tr.position, castDirection),sphereRadius, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore))
        {
            return Mathf.Max(0f,hit.distance-minimumDistanceFrmoObstacles);
        }
        
        return castDirection.magnitude;
    }
}
