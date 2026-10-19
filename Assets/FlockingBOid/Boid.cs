using UnityEngine;

public class Boid : MonoBehaviour
{

    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float maxForce = 34f;
    [SerializeField] float Perception = 2.5f;

    [SerializeField] LayerMask boidMask;

    public FlockManager Flock { get; set; }
    public Vector3 Velocity { get; private set; }
    public float MaxSpeed => maxSpeed;
    public float MaxForce => maxForce;

    Rigidbody rb;
    readonly Collider[] near = new Collider[32];

    private void Awake() => rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, Perception, near, boidMask);
        Vector3 force = Flock.ComputeForce(this, near, count);


        Velocity= Vector3.ClampMagnitude(Velocity+force*Time.fixedDeltaTime, maxSpeed);
        rb.linearVelocity = Velocity;
        if (Velocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(Velocity);
        }
    }



}
