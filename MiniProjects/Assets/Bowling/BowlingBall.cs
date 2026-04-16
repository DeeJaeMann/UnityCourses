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
        Debug.Log("Bowl");
    }
}
