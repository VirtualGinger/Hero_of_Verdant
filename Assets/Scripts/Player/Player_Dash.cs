using UnityEngine;

public class Player_Dash : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;

    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private Vector2 dashDir;

    public bool IsDashing => isDashing;

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            rb.linearVelocity = dashDir * dashSpeed;

            if (dashTimer <= 0)
                StopDash();
        }
    }

    public void StartDash(Vector2 direction)
    {
        if (isDashing || cooldownTimer > 0)
            return;

        if (direction == Vector2.zero)
            direction = Vector2.down;

        dashDir = direction.normalized;

        anim.SetFloat("Horizontal", dashDir.x);
        anim.SetFloat("Vertical", dashDir.y);
        anim.SetBool("IsDashing", true);

        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
    }

    private void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDashing", false);
    }
}