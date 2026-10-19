using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

public sealed class FlockManager : MonoBehaviour
{
    [SerializeField] Boid boidPrefab;

    [SerializeField] int count = 150;

    [SerializeField] float spawnRadius = 8f;

    public readonly List <Boid> boids = new();

    private void Awake()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;

            Boid boid = Instantiate(boidPrefab, pos,Quaternion.Euler(0f,Random.value*360f,0f));

            boid.Flock = this;
            boids.Add(boid);
        }
    }
}
