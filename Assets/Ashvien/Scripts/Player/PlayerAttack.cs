using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackSize = new Vector2(1.2f, 0.8f);
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 0.25f;

    private float attackTimer;
    private int facingDirection = 1;

    private void Update()
    {
        UpdateFacingDirection();

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    private void UpdateFacingDirection()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
        {
            facingDirection = 1;
        }
        else if (horizontalInput < 0)
        {
            facingDirection = -1;
        }

        if (attackPoint != null)
        {
            attackPoint.localPosition = new Vector3(
                Mathf.Abs(attackPoint.localPosition.x) * facingDirection,
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
    }

    private void Attack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0f,
            enemyLayer
        );

        Debug.Log($"Attack hit {enemiesHit.Length} enemies.");

        foreach (Collider2D enemy in enemiesHit)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                Vector2 hitDirection = (enemy.transform.position - transform.position).normalized;
                enemyHealth.TakeDamage(damage, hitDirection);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}