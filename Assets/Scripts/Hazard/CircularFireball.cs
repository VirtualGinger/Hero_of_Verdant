using UnityEngine;

public class CircularFireball : MonoBehaviour
{
    [Header("Movement Settings")]
    public float radius = 2f;
    public float speed = 100f; // Speed in degrees per second
    
    [Tooltip("Adjust this if the sprite is facing the wrong way (try 0, 90, 180, etc)")]
    public float rotationOffset = 0f; 

    [Header("Damage Settings")]
    public int damageAmount = 1;

    private Vector3 centerPoint;
    private float angle;

    private void Start()
    {
        // Lock the center point to where you placed the object
        centerPoint = transform.position;
    }

    private void Update()
    {
        // 1. Increment angle based on time
        angle += speed * Time.deltaTime;

        // 2. Calculate offsets using Sin/Cos
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        // 3. Apply position
        transform.position = centerPoint + new Vector3(x, y, 0);

        // 4. Apply rotation (Orbiting rotation)
        // We add the offset so you can tweak it to face forward/inward/outward
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit object named: " + other.name + " | Tag: " + other.tag);
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