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
        anim.SetFloat("AttackX", direction.x);
        anim.SetFloat("AttackY", direction.y);

        if (timer <= 0)
        {
            anim.SetBool("IsAttacking", true);
            timer = cooldown;
        }

        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (Collider2D enemyGameobject in enemy)
        {
            Debug.Log("Ememy Hit");
            enemyGameobject.GetComponent<EnemyHealth>().health -= attackDamage;
        }
    }

//public void attack()
  //  {

//    }



    public void FinishAttack()
    {
        anim.SetBool("IsAttacking", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}
