using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float detectionAngle = 60f;
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] float innerDetectionRadius = 5f;
    [SerializeField] float detectionCooldown = 0.5f;
    [SerializeField] float attackRange = 2f;

    public Transform player { get; private set; }
    CountdownTimer detectionTimer;
    Collider[] hitColliders;
    public LayerMask playerMask;
    IdetectionStartegy detectionStrategy;

    private void Awake()
    {
        hitColliders = new Collider[1];
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        detectionTimer = new CountdownTimer(detectionCooldown);      
        detectionStrategy = new ConeDetectionStrategy(detectionAngle,detectionRadius, innerDetectionRadius);
    }

    private void Update()
    {
        detectionTimer.Tick();
    }

    public bool CanDetectPlayer()
    {
        return detectionTimer.IsRunning || detectionStrategy.Execute(player, transform, detectionTimer);
    }

    public bool CanAttackPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        return directionToPlayer.magnitude <= attackRange;
    }

    public bool isInRadius()
    {
        int playerInt=Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, hitColliders, playerMask);
        return playerInt > 0;
    }

    public void SetDetectionStrategy(IdetectionStartegy startegy)=>this.detectionStrategy = startegy;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 forwardConeDir = Quaternion.Euler(0, detectionAngle / 2, 0)*transform.forward*detectionRadius;
        Vector3 backwardConeDir = Quaternion.Euler(0, -detectionAngle / 2, 0)*transform.forward*detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position+forwardConeDir);
        Gizmos.DrawLine(transform.position, transform.position+backwardConeDir);
    }
}
