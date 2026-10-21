using System.Collections.Generic;
using UnityEngine;

public sealed class FlockManager : MonoBehaviour
{
    [SerializeField] Boid boidPrefab;
    [SerializeField] int count = 150;
    [SerializeField] float spawnRadius = 8f;

    [Header("Pesos")]
    [SerializeField] float sepWeight = 1.6f;
    [SerializeField] float aliWeight = 1f;
    [SerializeField] float cohWeight = 0.9f;

    [Header("Mundo / obstáculos")]
    [SerializeField] float boundsRadius = 18f;
    [SerializeField] float lookAhead = 2.5f;
    [SerializeField] LayerMask obstacleMask;

    [Header("Predador (Parte B)")]
    [SerializeField] Transform predator;
    [SerializeField] float fleeRadius = 5f;
    [SerializeField] float fleeWeight = 4f;

    public readonly List<Boid> boids = new();
    Vector3 center;

    void Awake()
    {
        center = transform.position;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = center + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(boidPrefab, pos,
                Quaternion.Euler(0f, Random.value * 360f, 0f));
            boid.Flock = this;
            boids.Add(boid);
        }
    }

    public Vector3 ComputeForce(Boid boid, Collider[] near, int count)
    {
        Vector3 pos = boid.transform.position;
        Vector3 sep = Vector3.zero, ali = Vector3.zero, coh = Vector3.zero;
        int seen = 0;

        for (int i = 0; i < count; i++)
        {
            Collider col = near[i];
            if (col == null || col.attachedRigidbody == null ||
                col.gameObject == boid.gameObject) continue;

            Vector3 toMe = pos - col.transform.position;
            float d2 = toMe.sqrMagnitude + 0.0001f;
            sep += toMe.normalized / d2;               // 1/d²
            ali += col.attachedRigidbody.linearVelocity;
            coh += col.transform.position;
            seen++;
        }

        Vector3 force = Vector3.zero;
        if (seen > 0)
        {
            ali /= seen;
            coh = coh / seen - pos;
            if (sep.sqrMagnitude > 0f)
                force += (sep.normalized * boid.MaxSpeed - boid.Velocity) * sepWeight;
            force += (ali.normalized * boid.MaxSpeed - boid.Velocity) * aliWeight;
            force += (coh.normalized * boid.MaxSpeed - boid.Velocity) * cohWeight;
        }

        // sempre aplicados, mesmo sem vizinhos
        force += StayInBounds(boid);
        force += AvoidObstacle(boid) * 2f;
        force += Flee(boid) * fleeWeight;
        return force;
    }

    Vector3 StayInBounds(Boid boid)
    {
        Vector3 toCenter = center - boid.transform.position;
        if (toCenter.sqrMagnitude < boundsRadius * boundsRadius)
            return Vector3.zero;
        return toCenter.normalized * boid.MaxSpeed - boid.Velocity;
    }

    Vector3 AvoidObstacle(Boid boid)
    {
        if (boid.Velocity.sqrMagnitude < 0.0001f) return Vector3.zero;
        Vector3 dir = boid.Velocity.normalized;
        if (!Physics.SphereCast(boid.transform.position, 0.4f, dir,
                out RaycastHit hit, lookAhead, obstacleMask))
            return Vector3.zero;
        Vector3 away = Vector3.Reflect(dir, hit.normal);
        return away * boid.MaxSpeed - boid.Velocity;
    }

    Vector3 Flee(Boid boid)
    {
        if (predator == null) return Vector3.zero;
        Vector3 away = boid.transform.position - predator.position;
        if (away.sqrMagnitude > fleeRadius * fleeRadius) return Vector3.zero;
        return away.normalized * boid.MaxSpeed - boid.Velocity;
    }
}
