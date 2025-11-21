using UnityEngine;

public class EnemyDamage : Damage
{
    private Animator _parentAnim;
    private Collider2D _myCollider;

    [Header("Timing Settings")]
    [Range(0f, 1f)]
    public float startDamageTime = 0.3f; // Hitbox turns ON at 30%
    [Range(0f, 1f)]
    public float endDamageTime = 0.6f;   // Hitbox turns OFF at 60%

    void Awake()
    {
        _parentAnim = GetComponentInParent<Animator>();
        _myCollider = GetComponent<Collider2D>();
        
        // Safety: Ensure collider starts OFF so you don't get "Cactus" damage
        if (_myCollider != null) _myCollider.enabled = false;
    }

    void Update()
    {
        if (_parentAnim == null || _myCollider == null) return;

        AnimatorStateInfo stateInfo = _parentAnim.GetCurrentAnimatorStateInfo(0);

        // 1. Are we in the correct Attack State?
        if (stateInfo.IsName("Attack_State_Name"))
        {
            float time = stateInfo.normalizedTime % 1; // Handle looping just in case

            // 2. OPEN WINDOW: Turn Collider ON
            if (time >= startDamageTime && time <= endDamageTime)
            {
                _myCollider.enabled = true;
            }
            // 3. CLOSE WINDOW: Turn Collider OFF
            else
            {
                _myCollider.enabled = false;
                hasHit = false; // Reset hit flag when window closes
            }
        }
        else
        {
            // If not attacking, Collider is ALWAYS OFF
            _myCollider.enabled = false;
            hasHit = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // We use Enter instead of Stay because the Collider is flashing On/Off now
        if (other.gameObject.CompareTag("Player") && !hasHit)
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                hasHit = true; // Prevent double hits in the same window
                Debug.Log("PLAYER HIT! (Synced)");
            }
        }
    }
}