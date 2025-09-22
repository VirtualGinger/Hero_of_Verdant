using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movementt;

    void Update()
    {
        movementt.x = Input.GetAxisRaw("Horizontal");
        movementt.y = Input.GetAxisRaw("Vertical");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementt * moveSpeed * Time.fixedDeltaTime);
    }
}
