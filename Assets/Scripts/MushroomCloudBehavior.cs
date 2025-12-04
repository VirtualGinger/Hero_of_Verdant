using UnityEngine;
using System.Collections;

public class MushroomCloudBehavior : MonoBehaviour
{
    [Header("Cloud Settings")]
    public float startDelay = 0.2f;       // Small delay so cloud doesn't pop instantly when animation starts
    public float fullSize = 3.0f;         // How big the cloud gets
    public float growSpeed = 5.0f;        // How fast it grows
    public float lingerTime = 1.0f;       // How long it stays before disappearing
    public int damageAmount = 10;         // Amount of damage to deal

    [Header("Debug")]
    public bool isAttacking = false;

    private Enemy_behavior parentEnemy;
    private Animator parentAnim;
    private SpriteRenderer cloudSprite;
    private CircleCollider2D cloudCollider;

    void Awake()
    {
        parentEnemy = GetComponentInParent<Enemy_behavior>();
        parentAnim = GetComponentInParent<Animator>();


        cloudSprite = GetComponent<SpriteRenderer>();
        cloudCollider = GetComponent<CircleCollider2D>();


        ResetCloud();
    }

    void Update()
    {

        if (parentAnim == null) return;

        if (parentAnim.GetBool("Attack") == true && !isAttacking)
        {
            StartCoroutine(SpawnCloudRoutine());
        }
    }

    IEnumerator SpawnCloudRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(startDelay);

        if(cloudSprite) cloudSprite.enabled = true;
        if(cloudCollider) cloudCollider.enabled = true;
        transform.localScale = Vector3.zero; 


        while (transform.localScale.x < fullSize)
        {
            transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(lingerTime);

        ResetCloud();

        if (parentEnemy != null)
        {
            parentEnemy.TriggerCooling();
        }

        while (parentAnim.GetBool("Attack") == true)
        {
            yield return null;
        }

        isAttacking = false;
    }

    void ResetCloud()
    {
        if(cloudSprite) cloudSprite.enabled = false;
        if(cloudCollider) cloudCollider.enabled = false;
        transform.localScale = Vector3.zero;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}