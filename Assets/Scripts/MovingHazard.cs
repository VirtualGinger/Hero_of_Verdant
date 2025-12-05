using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    [Header("Hazard Stats")]
    public int damageAmount = 1;

    [Header("Movement Settings")]
    public float speed = 3f;           // How fast it moves
    public float distance = 4f;        // How far it moves from the start point
    public bool moveVertically = false;// Check this box to move Up/Down instead of Left/Right
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 200f; // How fast it spins (degrees per second)

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);


        float movement = Mathf.PingPong(Time.time * speed, distance);

        if (moveVertically)
        {

            transform.position = new Vector3(startPos.x, startPos.y + movement, startPos.z);
        }
        else
        {

            transform.position = new Vector3(startPos.x + movement, startPos.y, startPos.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}