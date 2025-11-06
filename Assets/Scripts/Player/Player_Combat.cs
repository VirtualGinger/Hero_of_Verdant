using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = 1;
    
    private float timer;


    public Transform attackPoint;
    public float radius;
    public LayerMask enemies;
    public float attackDamage = 5;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

public void Attack(Vector2 direction)
{
    anim.SetFloat("AttackX", direction.x);
    anim.SetFloat("AttackY", direction.y);

    if (timer <= 0)
    {
        anim.SetBool("IsAttacking", true);
        timer = cooldown;

        Vector3 attackPos = transform.position;

        if (direction == Vector2.up)
            attackPos += Vector3.up * 1f;
        else if (direction == Vector2.down)
            attackPos += Vector3.down * 1f;
        else if (direction == Vector2.left)
            attackPos += Vector3.left * 1f;
        else if (direction == Vector2.right)
            attackPos += Vector3.right * 1f;
        else
            attackPos += new Vector3(direction.x, direction.y, 0).normalized * 1f;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, radius, enemies);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Enemy Hit: " + enemy.name);

            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.health -= attackDamage;
            }
        }
    }
}
    
public void FinishAttack()
{
    anim.SetBool("IsAttacking", false);
}

//public void attack()
  //  {

//    }



private void OnDrawGizmosSelected()
{
    if (attackPoint != null)
    {
        Gizmos.color = Color.red;

        Vector3 attackPos = attackPoint.transform.position;
        Gizmos.DrawWireSphere(attackPos, radius);
    }
}
}
