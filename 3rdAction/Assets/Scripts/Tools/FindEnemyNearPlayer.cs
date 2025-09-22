using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class FindEnemyNearPlayer : MonoBehaviour
{
    
    [SerializeField] private int maxColliders = 10;
    [SerializeField] private float sphereRadius = 15f;
    Collider[] hitColliders;
    public LayerMask enemyMask;
    

    

    private void Awake()
    {
        hitColliders = new Collider[maxColliders];
        
    }


    public Transform FindCloseestEnemy()
    {
        
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, hitColliders,enemyMask);
        Transform bestTarget = null;
        Vector3 currentPosition = transform.position;
        float ClosestDistance = float.MaxValue;

        for (int i = 0;i< numColliders;i++)
        {
            if (!EnemyManager.Contains(hitColliders[i].gameObject)||!OnScreen(hitColliders[i].gameObject))
                continue;
            Vector3 DifferenceToTarget = hitColliders[i].gameObject.transform.position - currentPosition;
            float DistanceToTarget = DifferenceToTarget.sqrMagnitude;

            if (DistanceToTarget < ClosestDistance)
            {
                ClosestDistance = DistanceToTarget;
                bestTarget = hitColliders[i].gameObject.transform;
            }
        }

        return bestTarget;
    }

    bool OnScreen(GameObject enemy)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return planes.All(plane => plane.GetDistanceToPoint(enemy.transform.position) >= 0);
    }
  

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, sphereRadius);
    }*/
}
