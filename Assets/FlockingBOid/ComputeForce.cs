using UnityEngine;

[SerializeField] float sepWeight = 1.6f;
[SerializeField] float aliWeight = 1f;
[SerializeField] float cohWeight = 0.9f;

public Vector3 ComputeForce(Boid boid, Collider[] near, int count)
{
    Vector3 sep = Vector3.zero;
    Vector3 ali = Vector3.zero;
    Vector3 coh = Vector3.zero;

    int seen = 0;

    for (int i = 0; i < count; i++)
    {
        Collider col = near[i];

        if (col == null ||
            col.attachedRigidbody == null ||
            col.gameObject == boid.gameObject)
            continue;

        Vector3 toMe =
            boid.transform.position - col.transform.position;

        // Separação
        sep += toMe / (toMe.sqrMagnitude + 0.0001f);

        // Alinhamento
        ali += col.attachedRigidbody.linearVelocity;

        // Coesão
        coh += col.transform.position;

        seen++;
    }

    if (seen == 0)
        return Vector3.zero;

    // Médias dos vizinhos
    ali /= seen;
    coh = coh / seen - boid.transform.position;

    // Forças desejadas
    Vector3 sepForce =
        sep.normalized * boid.MaxSpeed - boid.Velocity;

    Vector3 aliForce =
        ali.normalized * boid.MaxSpeed - boid.Velocity;

    Vector3 cohForce =
        coh.normalized * boid.MaxSpeed - boid.Velocity;

    // Combinação das três forças
    return sepForce * sepWeight
         + aliForce * aliWeight
         + cohForce * cohWeight;
}