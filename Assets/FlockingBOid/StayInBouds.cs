using UnityEngine;

public Vector3 StayInBounds(Boid boid)
{
    Vector3 toCenter = center - boid.transform.position;
    if (toCenter.sqrMagnitude < boundsRadius * boundsRadius)
        return Vector3.zero;
    Vector3 desiredVelocity = toCenter.normalized * boid.MaxSpeed;
    return Vector3.ClampMagnitude(
        desiredVelocity - boid.Velocity,
        boid.MaxForce
    );
}
public Vector3 AvoidObstacle(Boid boid)
{
    if (boid.Velocity.sqrMagnitude < 0.0001f)
        return Vector3.zero;
    Vector3 dir = boid.Velocity.normalized;
    if (!Physics.SphereCast(
            boid.transform.position,
            0.4f,
            dir,
            out RaycastHit hit,
            lookAhead,
            obstacleMask))
    {
        return Vector3.zero;
    }
    Vector3 away = Vector3.Reflect(dir, hit.normal);
    Vector3 desiredVelocity = away * boid.MaxSpeed;
    return Vector3.ClampMagnitude(
        desiredVelocity - boid.Velocity,
        boid.MaxForce
    );
}
