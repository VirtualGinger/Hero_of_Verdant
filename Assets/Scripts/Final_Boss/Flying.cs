using UnityEngine;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Movement & Flying")]
    public float flySpeed = 3f;
    public float minFlyTime = 2f;
    public float maxFlyTime = 5f;
    public float flyInterval = 5f;

    private bool isFlying = false;
    private float originalScaleX;
    private Vector3 targetPosition;

    private Animator anim;
    private EnemyHealth enemyHealth;
    private Enemy_behavior enemyBehavior;

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyBehavior = GetComponent<Enemy_behavior>();
        originalScaleX = Mathf.Abs(transform.localScale.x);

        StartCoroutine(FlyingCycle());
    }

    void Update()
    {
        if (isFlying)
        {
            FlyMovement();
        }
    }

    IEnumerator FlyingCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(flyInterval);

            isFlying = true;

            enemyHealth.enabled = false;
            if (enemyBehavior != null)
            {
                enemyBehavior.enabled = false;
            }

            anim.SetBool("isFlying", true);

            float flyDuration = Random.Range(minFlyTime, maxFlyTime);
            float timer = 0f;

            float direction = Random.value < 0.5f ? -1f : 1f;
            targetPosition = transform.position + new Vector3(direction * 3f, 1f, 0f);

            while (timer < flyDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            isFlying = false;
            enemyHealth.enabled = true;
            if (enemyBehavior != null)
            {
                enemyBehavior.enabled = true;
            }

            anim.SetBool("isFlying", false);
        }
    }

    void FlyMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flySpeed * Time.deltaTime);

        if (targetPosition.x > transform.position.x)
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
    }
}