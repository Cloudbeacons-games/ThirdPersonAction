using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RaycastSensor
{
    public float castLength = 1f;
    public LayerMask layerMask;

    Vector3 origin = Vector3.zero;
    Transform player;

    public enum CastDirection { Forward, Backward , Up, Down , Left , Right }
    CastDirection castDirection;

    RaycastHit hitInfo;

    public RaycastSensor(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void Cast()
    {
        Vector3 worldOrigin = player.TransformPoint(origin);
        Vector3 worldDirection = GetCastDirection();

        
        Physics.Raycast(worldOrigin,worldDirection,out hitInfo,castLength,layerMask,QueryTriggerInteraction.Ignore);


    }

    public bool HasDetectedHit()=>hitInfo.collider!= null;
    public float GetDistance()=>hitInfo.distance;
    public Vector3 GetNormal()=> hitInfo.normal;
    public Vector3 GetPosition()=> hitInfo.point;
    public Collider GerCollider()=> hitInfo.collider;
    public Transform GetTransform()=> hitInfo.transform;

    public void SetCastDirection(CastDirection direction) => castDirection = direction;
    public void SetCastOrigin(Vector3 pos)=> origin = player.InverseTransformPoint(pos);

    private Vector3 GetCastDirection()
    {
        return castDirection switch
        {
            CastDirection.Forward => player.forward,
            CastDirection.Backward => -player.forward,
            CastDirection.Up => player.up,
            CastDirection.Down => -player.up,
            CastDirection.Left => -player.right,
            CastDirection.Right => player.right,
            _ => Vector3.one
        };
    }

    public void DrawDebug()
    {
        Vector3 worldOrigin = player.TransformPoint(origin);
        Vector3 worldDirection = GetCastDirection();
        Debug.DrawRay(worldOrigin, worldDirection*castLength, Color.red, 1f);
    }


}
