using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float rotateSpeed = 300f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float rotDir = 0f;
        if (Input.GetButton("Fire1")) rotDir = -1f;
        if (Input.GetButton("Fire2")) rotDir = 1f;

        if (rotDir != 0f)
        {
            Quaternion deltaRot = Quaternion.Euler(0f, rotDir * rotateSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRot);
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0f && v == 0f) return;

        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }
}