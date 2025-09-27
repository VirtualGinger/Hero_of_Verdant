using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = 1;
    private float timer;

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
    }

    public void FinishAttack()
    {
        anim.SetBool("IsAttacking", false);
    }
}
