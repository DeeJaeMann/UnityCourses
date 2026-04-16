using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float strafeForce = 2.0f;
    [SerializeField] private Rigidbody rig;

    void FixedUpdate()
    {
        float input = 0.0f;

        if (Keyboard.current.leftArrowKey.isPressed) input = -1.0f;
        if (Keyboard.current.rightArrowKey.isPressed) input = 1.0f;
        
        // Debug.Log($"input: {input}");
        rig.AddForce(transform.right * (input * strafeForce));
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"Collision with {collision.collider.gameObject.name}");
        if (collision.collider.gameObject.CompareTag("Tree"))
        {
            // Debug.Log("Hit a tree");   
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}
