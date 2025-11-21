using UnityEngine;

public class Enemy_behavior_archer : MonoBehaviour
{
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    public float verticalTolerance = 0.5f;

    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    private float intTimer;
    private bool isFacingRight = true;
    private EnemyHealth _enemyHealth;

    private ArrowLauncher arrowLauncher;
    private SpriteRenderer sprite;

    // Reference to the child launch point
    public Transform launchPoint;

    void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
        _enemyHealth = GetComponent<EnemyHealth>();
        arrowLauncher = GetComponent<ArrowLauncher>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_enemyHealth.health <= 0)
        {
            anim.SetBool("canWalk", false);
            anim.SetBool("Attack", false);
            return;
        }

        if (inRange && target != null)
        {
            float targetX = target.transform.position.x;
            Vector2 direction = (targetX < transform.position.x) ? Vector2.left : Vector2.right;

            Flip(targetX);

            hit = Physics2D.Raycast(rayCast.position, direction, rayCastLength, raycastMask);
            RaycastDebugger();

            if (hit.collider != null)
            {
                EnemyLogic();
            }
            else
            {
                inRange = false;
            }
        }
        else
        {
            inRange = false;
        }

        if (!inRange)
        {
            anim.SetBool("canWalk", false);
            StopAttack();
            target = null;
        }

        if (cooling) Cooldown();
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            target = trig.gameObject;
            inRange = true;
        }
    }

    void EnemyLogic()
    {
        if (target == null) return;

        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);
        if (target == null) return;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            float yDifference = Mathf.Abs(target.transform.position.y - transform.position.y);

            Vector2 targetPosition = (yDifference > verticalTolerance)
                ? new Vector2(transform.position.x, target.transform.position.y)
                : new Vector2(target.transform.position.x, transform.position.y);

            // Flip while walking
            Flip(target.transform.position.x);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public Vector2 GetFacingDirection()
    {
        
        return isFacingRight ? Vector2.right : Vector2.left;
    }

    void Attack()
    {
        if (target == null) return;

        Flip(target.transform.position.x); // ensure facing before firing

        attackMode = true;
        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);

        if (arrowLauncher != null)
        {
            
            arrowLauncher.FireProjectile(GetFacingDirection());
        }

        TriggerCooling();
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            cooling = false;
            attackMode = false;
            timer = intTimer;
        }

        if (cooling) anim.SetBool("Attack", false);
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
        timer = intTimer;
    }

    // Flip both sprite and launchPoint
    void Flip(float targetX)
    {
        bool shouldFaceRight = targetX > transform.position.x;

        if (shouldFaceRight && !isFacingRight)
        {
            isFacingRight = true;
            sprite.flipX = true; // facing right (inverted if needed)

            if (launchPoint != null)
            {
                Vector3 lpScale = launchPoint.localScale;
                launchPoint.localScale = new Vector3(Mathf.Abs(lpScale.x), lpScale.y, lpScale.z);
            }
        }
        else if (!shouldFaceRight && isFacingRight)
        {
            isFacingRight = false;
            sprite.flipX = false; // facing left

            if (launchPoint != null)
            {
                Vector3 lpScale = launchPoint.localScale;
                launchPoint.localScale = new Vector3(-Mathf.Abs(lpScale.x), lpScale.y, lpScale.z);
            }
        }
    }

    void RaycastDebugger()
    {
        if (target == null) return;
        Vector2 direction = (target.transform.position.x < transform.position.x) ? Vector2.left : Vector2.right;

        if (distance > attackDistance)
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.red);
        else if (attackDistance > distance)
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.green);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }
}
