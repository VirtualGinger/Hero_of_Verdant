using UnityEngine;

public class Arrow : MonoBehaviour
{

    public int damage = 5;
    public Vector2 moveSpeed = new Vector2(3f, 0);

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector2(moveSpeed * transform.localScale.x, moveSpeed.y);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            bool gotHit = damageable.Hit(damage);

            if(gotHit)
            {
                Debug.Log("arrow hit");
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
