using UnityEngine;
using System.Collections;

public class playerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public HealthUI healthUI;
    
    private Animator animator;
    private float hitResetTime = 0.35f; // Adjust as needed for your animation

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LightBanditMovement bandit = collision.gameObject.GetComponent<LightBanditMovement>();
        if (bandit != null)
        {
            // Set the bandit's attack animation
            Animator LightBanditAnimator = collision.gameObject.GetComponent<Animator>();
            if (LightBanditAnimator != null)
            {
                LightBanditAnimator.SetBool("isAttacking", true);
                StartCoroutine(ResetIsAttacking(LightBanditAnimator, 0.5f));
            }

            // The object is a bandit, so take damage
            TakeDamage(bandit.damage);
        }
    }
    private IEnumerator ResetIsAttacking(Animator LightBanditAnimator, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (LightBanditAnimator != null)
        {
            LightBanditAnimator.SetBool("isAttacking", false);
        }
    }
    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);
        
        if (animator != null)
        {
            animator.SetBool("isHit", true);
            StartCoroutine(ResetIsHit());
        }

        if (currentHealth <= 0)
        {
            //Die();
        }
    }

    private System.Collections.IEnumerator ResetIsHit()
    {
        yield return new WaitForSeconds(hitResetTime);
        if (animator != null)
        {
            animator.SetBool("isHit", false);
        }
    }
}
