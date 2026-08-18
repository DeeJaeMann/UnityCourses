using UnityEngine;
using UnityEngine.InputSystem;

public class Driver : MonoBehaviour
{
    [SerializeField] float steerSpeed = 200f;
    [SerializeField] float moveSpeed = 10f;
    
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

        float moveAmount = move * moveSpeed * Time.deltaTime;
        float steerAmount = steer * steerSpeed * Time.deltaTime;
        
        transform.Rotate(0, 0, steerAmount);
        transform.Translate(0, moveAmount, 0);
    }
}
