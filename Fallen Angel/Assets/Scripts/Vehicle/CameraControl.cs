using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform carLookatTransform;
    [SerializeField] Transform carTransform;

    Vector3 velocity = Vector3.zero;
    [SerializeField] float smoothingTime = 1f;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        transform.LookAt(carTransform);
        transform.position = Vector3.SmoothDamp(transform.position, carLookatTransform.position, ref velocity, smoothingTime * Time.deltaTime);
        
    }
}
