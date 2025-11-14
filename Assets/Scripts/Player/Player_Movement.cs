using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    bool isDashing = false;
    float dashDuration = 1f;
    float dashCooldown = 1f;
    public Rigidbody2D rb;
    public Animator animator;
    Vector2 movement;
    private Vector2 LastDirection = Vector2.down;
    public Player_Combat player_Combat;
    

    private void Update()
    {
    if (Input.GetButtonDown("Slash"))
    {
        player_Combat.Attack(LastDirection);
    }

    if (Input.GetButtonDown("Dash") && dashCooldown <= 0f && !isDashing)
    {
        StartCoroutine(Dash());
    }

    if (dashCooldown > 0f)
    {
        dashCooldown -= Time.deltaTime;
    }
    }


    void FixedUpdate()
    {
        if (isDashing) return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
        {
            LastDirection = movement;
        }

        player_Combat.attackPoint.transform.localPosition = LastDirection.normalized * 0.75f;

        animator.SetFloat("Horizontal", LastDirection.x);
        animator.SetFloat("Vertical", LastDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator Dash()
{
    isDashing = true;

    Vector2 dashDirection = LastDirection.normalized;

    while (dashDuration > 0f)
    {
        rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
        dashDuration -= Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();
    }

    isDashing = false;
}


}
