using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockbackForce = 6f;

    private int currentHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHealth}");

        if (rb != null)
        {
            rb.velocity = new Vector2(hitDirection.x * knockbackForce, 3f);
        }

        if (HitStop.Instance != null)
        {
            HitStop.Instance.Stop(0.06f);
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}