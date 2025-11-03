using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = 1;
    
    private float timer;


    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    public float attackDamage;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack(Vector2 direction)
    {
        // Update attack direction for animations
        anim.SetFloat("AttackX", direction.x);
        anim.SetFloat("AttackY", direction.y);

        // Check if cooldown is done
        if (timer <= 0)
        {
            anim.SetBool("IsAttacking", true);
            timer = cooldown;

            // Detect enemies in attack radius
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Enemy Hit: " + enemy.name);

                // Safely apply damage (only if enemy has EnemyHealth)
                EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
                if (hp != null)
                {
                    hp.health -= attackDamage;
                }
            }
        }
    }
    


//public void attack()
  //  {

//    }



    public void FinishAttack()
    {
        anim.SetBool("IsAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        }
    }
}
