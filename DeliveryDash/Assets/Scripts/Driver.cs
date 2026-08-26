using UnityEngine;
using UnityEngine.InputSystem;

public class Driver : MonoBehaviour
{
    [SerializeField] private float steerSpeed = 200f;
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float defaultSpeed = 5f;
    private bool _hasBoost;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float steer = 0f;
        float move = 0f;
        
        // forward / backward
        if (Keyboard.current.wKey.isPressed)
        {
            // Debug.Log("Forward pressed");
            move = 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            // Debug.Log("Backward pressed");
            move = -1f;
        }
        
        // left / right
        if (Keyboard.current.aKey.isPressed)
        {
            // Debug.Log("Left pressed");
            steer = 1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            // Debug.Log("Right pressed");
            steer = -1f;
        }

        float moveAmount = move * currentSpeed * Time.deltaTime;
        float steerAmount = steer * steerSpeed * Time.deltaTime;
        
        transform.Rotate(0, 0, steerAmount);
        transform.Translate(0, moveAmount, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Triggered {other.name}");
        if (other.CompareTag("Boost") && !_hasBoost)
        {
            Debug.Log($"Boost Trigger {other.name}");
            currentSpeed = boostSpeed;
            Destroy(other.gameObject);
            _hasBoost = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        currentSpeed = defaultSpeed;
    }
}
