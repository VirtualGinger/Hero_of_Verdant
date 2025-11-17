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
    public Player_Dash player_Dash;
    

    private void Update(){
    if (Input.GetButtonDown("Slash")){
        player_Combat.Attack(LastDirection);
    }
    if (Input.GetButtonDown("Dash")){
        player_Dash.StartDash(LastDirection);
    }
        if (Input.GetButtonDown("Dash"))
        {
            Debug.Log("DASH BUTTON PRESSED");
        }

    }


    void FixedUpdate()
{
    // DO NOT MOVE WHILE DASHING
    if (player_Dash.IsDashing)
        return;

    Vector2 movement;
    movement.x = Input.GetAxisRaw("Horizontal");
    movement.y = Input.GetAxisRaw("Vertical");

    // Update LastDirection only when moving
    if (movement.sqrMagnitude > 0.01f)
        LastDirection = movement.normalized;

    // Animate
    animator.SetFloat("Horizontal", LastDirection.x);
    animator.SetFloat("Vertical", LastDirection.y);
    animator.SetFloat("Speed", movement.sqrMagnitude);

    // Move player normally
    rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
}


}
