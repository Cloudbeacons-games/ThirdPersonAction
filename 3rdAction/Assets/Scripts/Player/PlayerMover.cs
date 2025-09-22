using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMover : MonoBehaviour
{
    #region Fields
    [Header("Collider Settings:")]
    [Range(0f, 1f)][SerializeField] float stepHeightRatio = 0.1f;
    [SerializeField] float colliderHeight = 2f;
    [SerializeField] float colliderThickness = 1f;
    [SerializeField] Vector3 colliderOffset = Vector3.zero;

    Rigidbody rb;
    Transform tr;
    CapsuleCollider col;
    RaycastSensor sensor;

    bool isGrounded;
    float baseSensorRange;
    Vector3 currentGroundAdjustmenVelocity;//velocity to adjust player pos to maintain gound contact
    int currentLayer;

    [Header("Sensor Settings:")]
    [SerializeField] bool isInDebugMode;
    bool isUsingExtendedSensorRange = true; //use extended range for smoother groun transitions
    #endregion

    private void Awake()
    {
        Setup();
        RecalculateColliderDimension();
    }

    private void OnValidate()
    {
        if(gameObject.activeInHierarchy)
        {
            RecalculateColliderDimension();
        }
    }

    void LateUpdate()
    {
        if (isInDebugMode)
        {
           sensor.DrawDebug();
        }
    }

    public void CheckForGround()
    {
        if(currentLayer != gameObject.layer)
        {
            RecalculateSensorLayerMask();
        }

        currentGroundAdjustmenVelocity = Vector3.zero;
        sensor.castLength = isUsingExtendedSensorRange
            ? baseSensorRange + colliderHeight * tr.localScale.x * stepHeightRatio
            : baseSensorRange;
        sensor.Cast();
        isGrounded = sensor.HasDetectedHit();

        if(!isGrounded)
            return;
        float distance = sensor.GetDistance();
        float upperLimit = colliderHeight * tr.localScale.x * (1f * stepHeightRatio) * 0.5f;
        float middle = upperLimit + colliderHeight * tr.localScale.x*stepHeightRatio;
        float distanceToGo = middle - distance;

        currentGroundAdjustmenVelocity = tr.up*(distanceToGo/Time.fixedDeltaTime);
        
    }

    public bool IsGrounded() => isGrounded;
    public Vector3 GetGroundNormal()=>sensor.GetNormal();

    public LayerMask GetGroundLayer() => sensor.GerCollider().gameObject.layer;

    public void SetVelocity(Vector3 velocity)=> rb.linearVelocity = velocity+currentGroundAdjustmenVelocity;
    public void SetExtendedSensorRange(bool isExtended)=> isUsingExtendedSensorRange = isExtended;
    private void Setup()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    private void RecalculateSensorLayerMask()
    {
        int objLayer = gameObject.layer;
        int layerMask = Physics.AllLayers;
        for (int i =0;i<32;i++)
        {
            if(Physics.GetIgnoreLayerCollision(objLayer,i))
            {
                layerMask &= ~(1 << i);
            }
        }

        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        layerMask &= ~(1 << ignoreRaycastLayer);

        sensor.layerMask = layerMask;
        currentLayer = objLayer;
    }
    private void RecalibrateSensor()
    {
        sensor ??= new RaycastSensor(tr);
        sensor.SetCastOrigin(col.bounds.center);
        sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
        RecalculateSensorLayerMask();

        const float safetyDistanceFactor = 0.001f;//small factor added to precent clipping issues when the sensor range is calculated

        float length = colliderHeight *(1f-stepHeightRatio)*0.4f + colliderHeight*stepHeightRatio;
        baseSensorRange = length * (1f + safetyDistanceFactor) * tr.localScale.x;
        sensor.castLength = length*tr.localScale.x;
        
    }
    private void RecalculateColliderDimension()
    {
        if (col == null)
            Setup();
        col.height = colliderHeight * (1f - stepHeightRatio);
        col.radius = colliderThickness / 2f;
        col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);

        if (col.height / 2f < col.radius)
            col.radius = col.height / 2f;

        RecalibrateSensor();
    }
}
