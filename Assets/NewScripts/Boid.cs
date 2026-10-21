using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    [SerializeField] float maxSpeed   = 8f;
    [SerializeField] float maxForce   = 24f;
    [SerializeField] float perception = 2.5f;
    [SerializeField] LayerMask boidMask;
    [SerializeField] bool drawGizmo;          // marque em 1 boid

    public FlockManager Flock { get; set; }
    public Vector3 Velocity { get; private set; }
    public float MaxSpeed => maxSpeed;
    public float MaxForce => maxForce;
    public float Perception { get => perception; set => perception = value; }

    Rigidbody rb;
    readonly Collider[] near = new Collider[32];

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void Start() => Velocity = transform.forward * maxSpeed * 0.5f;

    void FixedUpdate()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position, perception, near, boidMask);

        Vector3 force = Flock.ComputeForce(this, near, count);
        force = Vector3.ClampMagnitude(force, maxForce);

        Vector3 v = Vector3.ClampMagnitude(
            Velocity + force * Time.fixedDeltaTime, maxSpeed);

        if (float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z))
            v = transform.forward * maxSpeed * 0.5f;   // anti-NaN

        Velocity = v;
        rb.linearVelocity = Velocity;

        if (Velocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(Velocity);
    }

    void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, perception);
    }
}
