using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] float smoothSpeed = 10f;
    [SerializeField] Vector3 offset = new Vector3(0, 6, -8);

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
    }
}
