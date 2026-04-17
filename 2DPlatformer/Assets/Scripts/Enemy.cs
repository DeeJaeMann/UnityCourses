using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;

    private Vector3 targetPosition;
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPosition;
        targetPosition = endPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            if (targetPosition == startPosition)
            {
                targetPosition = endPosition;
            }
            else
            {
                targetPosition = startPosition;
            }
        }
        
        float xDirection = targetPosition.x - transform.position.x;
        spriteRenderer.flipX = xDirection < 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Debug.Log($"Hit {collision.gameObject.name}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
