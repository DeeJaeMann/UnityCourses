using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothing;

    // Update is called once per frame
    void Update()
    {
        // Vector3 newPosition = target.position +offset;
        Vector3 newPosition = Vector2.Lerp(transform.position, target.position + offset, Time.deltaTime *smoothing);
        newPosition.z = -10;

        transform.position = newPosition;
    }
}
