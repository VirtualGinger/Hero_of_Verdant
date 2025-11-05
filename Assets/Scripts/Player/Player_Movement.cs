using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
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
    }


    void FixedUpdate()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
        {
            LastDirection = movement;
        }

    // Move attackPoint in front of the player
        player_Combat.attackPoint.transform.localPosition = LastDirection.normalized * 0.75f;

        animator.SetFloat("Horizontal", LastDirection.x);
        animator.SetFloat("Vertical", LastDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
}
}
