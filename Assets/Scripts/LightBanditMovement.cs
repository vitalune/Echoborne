using UnityEngine;

public class LightBanditMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float jumpArcHeight = 2f; // Desired jump arc height
    public LayerMask groundLayer;
    public int damage = 1;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    private Vector2 jumpTarget;
    private Vector2 calculatedJumpVelocity;
    private float lastDirection = 1f;
    private float previousX;
    private float previousY;
    private Animator animator;

    // Checks if the bandit is on the ground
    private bool GroundCheck()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        previousX = transform.position.x;
        previousY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = GroundCheck();
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

        // Flip sprite if direction changes
        if (direction != 0 && direction != lastDirection)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -direction;
            transform.localScale = scale;
            lastDirection = direction;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            RaycastHit2D platformAhead = Physics2D.Raycast(transform.position + new Vector3(direction * 2f, 0, 0), Vector2.down, 5f, groundLayer);
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

            // Smart jump: if there's a gap or obstacle, calculate jump to next platform
            if ((!groundInFront.collider && !gapAhead.collider) || (groundInFront.collider && !groundInFront.collider.CompareTag("Player")))
            {
                if (platformAhead.collider)
                {
                    jumpTarget = platformAhead.point;
                    calculatedJumpVelocity = CalculateJumpVelocity(jumpTarget);
                    shouldJump = true;
                }
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                jumpTarget = platformAbove.point;
                calculatedJumpVelocity = CalculateJumpVelocity(jumpTarget);
                shouldJump = true;
            }
        }

        // Set isRunning to true if x is changing, false otherwise
        if (animator != null)
        {
            bool isRunning = Mathf.Abs(transform.position.x - previousX) > 0.01f;
            animator.SetBool("isRunning", true);

            // Set isJumping to true if y is changing, false otherwise, but set to false if isGrounded
            bool isJumping = Mathf.Abs(transform.position.y - previousY) > 0.01f;
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
            }
            else
            {
                animator.SetBool("isJumping", true);
            }
        }
        previousX = transform.position.x;
        previousY = transform.position.y;
    }

    private void FixedUpdate()
    {
        isGrounded = GroundCheck();
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            rb.AddForce(calculatedJumpVelocity, ForceMode2D.Impulse);
        }
    }

    // Calculate the required jump velocity to reach the target point
    private Vector2 CalculateJumpVelocity(Vector2 target)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        Vector2 start = rb.position;
        float dx = target.x - start.x;
        float dy = target.y - start.y;
        float h = jumpArcHeight;

        // Calculate time to reach the apex
        float vy = Mathf.Sqrt(2 * gravity * h);
        float t_up = vy / gravity;
        float t_down = Mathf.Sqrt(2 * (h - dy) / gravity);
        float t_total = t_up + t_down;
        float vx = dx / t_total;

        return new Vector2(vx, vy);
    }
}
