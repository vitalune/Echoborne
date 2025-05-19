using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private playerMovement movement;
    private float originalScaleX;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<playerMovement>();
        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        // Get vertical velocity for jump/fall states
        float verticalVelocity = movement.rb.linearVelocity.y;
        
        // Determine animation states
        bool isRunning = Mathf.Abs(movement.rb.linearVelocity.x) > 0.1f;
        bool isJumping = verticalVelocity > 0.1f;
        bool isFalling = verticalVelocity < -0.1f;
        
        // Set all animation parameters
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        
        // Handle character flipping based on movement direction
        if (movement.rb.linearVelocity.x > 0.1f)
        {
            // Face right but preserve scale
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (movement.rb.linearVelocity.x < -0.1f)
        {
            // Face left but preserve scale
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isAttacking");
        }
    }
}
