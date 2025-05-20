using UnityEngine;

public class LightBanditHealth : MonoBehaviour
{
    public float health = 3f;
    public float currentHealth;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = health;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            animator.SetTrigger("isHit");
        }
        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            Debug.Log("LightBandit is dead");

            // Disable movement script
            GetComponent<LightBanditMovement>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        }
    }
}
