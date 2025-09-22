using UnityEngine;

public interface IdetectionStartegy
{
    bool Execute(Transform player, Transform detector, CountdownTimer timer);
}

public class ConeDetectionStrategy : IdetectionStartegy
{
    readonly float detctionAngle;
    readonly float detectionRadius;
    readonly float innerDetectionRadius;

    public ConeDetectionStrategy(float detectionAngle,float detectionRadius,float innerDetectionRadius)
    {
        this.detctionAngle = detectionAngle;
        this.detectionRadius = detectionRadius;
        this.innerDetectionRadius = innerDetectionRadius;
    }
    public bool Execute(Transform player, Transform detector, CountdownTimer timer)
    {
        if(timer.IsRunning)
        {
            return false;
        }

        var directionToPlayer = player.position-detector.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);

        if((!(angleToPlayer < detctionAngle/2f)||!(directionToPlayer.magnitude<detectionRadius))&&!(directionToPlayer.magnitude<innerDetectionRadius))
        {
            return false;
        }

        timer.Start();
        return true;
    }
}