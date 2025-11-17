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
        if (attackTimer > 0f)
            return;

        attackTimer = cooldown;

        anim.SetBool("IsAttacking", true);
        anim.SetFloat("AttackX", direction.x);
        anim.SetFloat("AttackY", direction.y);

        Vector3 offset = GetAttackOffset(direction);
        Vector3 attackPos = transform.position + offset;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, radius, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
            if (hp != null)
                hp.health -= attackDamage;
        }
    }

    private Vector3 GetAttackOffset(Vector2 dir)
    {
        if (dir.y > 0.5f)
            return Vector3.up * 1f;
        else if (dir.y < -0.5f)
            return Vector3.down * 1f;
        else if (dir.x > 0.5f)
            return Vector3.right * 1f;
        else if (dir.x < -0.5f)
            return Vector3.left * 1f;

        return new Vector3(dir.x, dir.y, 0).normalized * 1f;
    }
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