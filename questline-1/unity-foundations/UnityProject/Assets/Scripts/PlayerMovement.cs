using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody rb;
    private Vector3 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movement = new Vector3(x, 0f, z).normalized;
    }

    void FixedUpdate()
    {
        // Preserve gravity while controlling horizontal movement.
#if UNITY_6000_0_OR_NEWER
        Vector3 velocity = rb.linearVelocity;
#else
        Vector3 velocity = rb.velocity;
#endif

        velocity.x = movement.x * speed;
        velocity.z = movement.z * speed;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }
}
