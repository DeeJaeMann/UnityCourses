using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private Rigidbody2D rig;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip scoreSound;
    private float moveInput;
    private bool jumpInput;
    private bool isGrounded;

    private int score;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        // Debug.Log($"Rig:{rig}");
    }

    void Start()
    {
        scoreText.text = $"Score: {score.ToString()}";
    }

    void FixedUpdate()
    {
        rig.linearVelocityX = moveInput * moveSpeed;

        CheckIfGrounded();
        
        if (jumpInput && isGrounded)
        {
            jumpInput = false;
            rig.linearVelocityY = jumpForce;
            audioSource.PlayOneShot(jumpSound);
        }

        if(moveInput is not 0) spriteRenderer.flipX = moveInput < 0;
        
        animator.SetBool("Moving", rig.linearVelocityX is not 0);
        animator.SetBool("Grounded", isGrounded);

        if (transform.position.y < -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);

        if (hit.collider is not null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // Debug.Log($"OnMoveInput: {context.phase}");
        moveInput = context.ReadValue<float>();
        // Debug.Log($"MoveInput: {moveInput}");
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Debug.Log($"OnJumpInput: {context.phase}");
        jumpInput = context.ReadValueAsButton();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        // Debug.Log($"Score: {score}");
        // scoreText.text = $"Score: {score.ToString()}";
        audioSource.PlayOneShot(scoreSound);
    }
}
