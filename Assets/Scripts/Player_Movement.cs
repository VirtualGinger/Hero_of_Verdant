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

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero){
            LastDirection = movement;
        }

        animator.SetFloat("Horizontal", LastDirection.x);
        animator.SetFloat("Vertical", LastDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
