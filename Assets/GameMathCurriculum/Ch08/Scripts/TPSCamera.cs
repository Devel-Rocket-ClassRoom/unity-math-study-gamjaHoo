using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 3f, -6f);

    public float positionSmoothTime = 0.15f;

    public float rotateSpeed = 8f;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + target.rotation * offset;
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPos, 
            ref _velocity, 
            positionSmoothTime);

        Vector3 lookDir = target.position - transform.position;

        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRot, 
            rotateSpeed * Time.deltaTime);
    }
}