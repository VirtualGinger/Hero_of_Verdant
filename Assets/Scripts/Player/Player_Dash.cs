using UnityEngine;

public class Player_Dash : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;

    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float cooldown = 1f;

    private float timer;
    private bool isDashing = false;
    private Vector2 dashDirection;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
    }

    public void Dash(Vector2 direction)
    {
        if (timer > 0 || isDashing)
            return;

        timer = cooldown;
        dashDirection = direction.normalized;

        anim.SetFloat("Vertical", direction.y);
        anim.SetFloat("Horizontal", direction.x);
        anim.SetBool("IsDashing", true);

        isDashing = true;

        Invoke(nameof(StopDash), dashDuration);
    }

    private void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDashing", false);
    }
}
