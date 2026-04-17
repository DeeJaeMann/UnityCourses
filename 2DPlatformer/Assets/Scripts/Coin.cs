using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float bobSpeed;
    [SerializeField] private float bobAmount;
    private float startYPosition;
    
    void Start()
    {
        startYPosition = transform.position.y;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobAmount / 2;
        Vector3 newPosition = transform.position;
        newPosition.y = startYPosition + offset;
        transform.position = newPosition;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Debug.Log($"Coin collided with {collision.gameObject.name}");
            collision.GetComponent<PlayerController>().IncreaseScore(1);
            Destroy(gameObject);
        }
    }
}
