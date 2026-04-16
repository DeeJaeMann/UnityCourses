using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    [SerializeField] private float bowlForce;
    [SerializeField] private float leftBarrier;
    [SerializeField] private float rightBarrier;
    [SerializeField] private float moveIncrement;
    [SerializeField] private Rigidbody rig;

    public void Bowl()
    {
        // Debug.Log("Bowl");
        rig.AddForce(Vector3.forward * bowlForce, ForceMode.Impulse);
    }

    public void MoveLeft()
    {
        // Debug.Log("MoveLeft");
        if(transform.position.x > leftBarrier) transform.position += Vector3.left * moveIncrement;
    }

    public void MoveRight()
    {
        // Debug.Log("MoveRight");
        if(transform.position.x < rightBarrier) transform.position += Vector3.right * moveIncrement;
    }
}
