using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [Header("Components")]
    public Animator anim;
    public Transform attackPoint;

    [Header("Attack Settings")]
    public float radius = 0.75f;
    public float attackDamage = 5f;
    public float cooldown = 0.5f;
    public LayerMask enemyLayers;

    private float attackTimer = 0f;
    public PlayerController playerController;

    void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    public void Attack(Vector2 direction)
    {
        // Enforce cooldown
        if (attackTimer > 0f)
            return;

        attackTimer = cooldown;

        // Update animation parameters
        anim.SetBool("IsAttacking", true);
        anim.SetFloat("AttackX", direction.x);
        anim.SetFloat("AttackY", direction.y);

        // Calculate attack origin relative to facing direction
        Vector3 offset = GetAttackOffset(direction);
        Vector3 attackPos = transform.position + offset;

        // Damage enemies in the radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, radius, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
            if (hp != null)
                hp.health -= attackDamage;
        }
    }

    /// <summary>
    /// Determines where the attack should be positioned based on direction.
    /// </summary>
    private Vector3 GetAttackOffset(Vector2 dir)
    {
        // Prioritize vertical direction
        if (dir.y > 0.5f)
            return Vector3.up * 1f;       // Up
        else if (dir.y < -0.5f)
            return Vector3.down * 1f;     // Down
        else if (dir.x > 0.5f)
            return Vector3.right * 1f;    // Right
        else if (dir.x < -0.5f)
            return Vector3.left * 1f;     // Left

        // If diagonal or small, use normalized direction
        return new Vector3(dir.x, dir.y, 0).normalized * 1f;
    }

    /// <summary>
    /// Called by Animation Event at end of attack animation.
    /// </summary>
    public void FinishAttack()
    {
        anim.SetBool("IsAttacking", false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}