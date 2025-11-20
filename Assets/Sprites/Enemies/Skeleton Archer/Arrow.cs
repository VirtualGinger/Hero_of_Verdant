using UnityEngine;

public class Arrow : MonoBehaviour
{

    public int damage = 5;
    public Vector2 moveSpeed = new Vector2(3f, 0);
    private GameObject target;


    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
            Debug.Log("Arrow hit Player");
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
