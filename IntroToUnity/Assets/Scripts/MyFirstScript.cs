using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Log("Hello!");

        // int health = 100;
        // Debug.Log($"Health: {health}");
        // health = 50;
        // Debug.Log($"Health: {health}");
        //
        // float moveSpeed = 5.25f;
        // Debug.Log($"MoveSpeed: {moveSpeed}");
        //
        // string playerName = "Bob";
        // Debug.Log($"PlayerName: {playerName}");
        //
        // bool gameOver = false;
        // Debug.Log($"GameOver: {gameOver}");

        // int score = 0;
        // score += 1;
        // Debug.Log($"Score: {score}");
        
        // LogToConsole();
        // Debug.Log($"Result: {Add(1, 2)}");

        // Vector3 newPosition = new(3, -2, 4);
        // Vector3 movement = new(1, 0, 0);
        // transform.position += movement;
    }

    // Update is called once per frame
    void Update()
    {
        // Per frame movement
        // Vector3 movement = new(1, 0, 0);
        // Time movement per second
        // var movement = new Vector3(1, 0, 0) * Time.deltaTime;
        // transform.position += movement;
        transform.position -= new Vector3(0, 1, 0) * Time.deltaTime;
    }

    void LogToConsole()
    {
        Debug.Log("This function has been called");
    }

    float Add(float a, float b)
    {
        return a + b;
    }
}
