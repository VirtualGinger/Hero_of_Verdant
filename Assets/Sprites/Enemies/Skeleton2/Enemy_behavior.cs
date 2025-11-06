using UnityEngine;

public class Enemy_behavior : MonoBehaviour
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
    private float originalScaleMagnitudeX;

    void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
        originalScaleMagnitudeX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
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

        if (inRange == false)
        {
            anim.SetBool("canWalk", false);
            StopAttack();
            target = null;
        }

        if (cooling)
        {
            Cooldown();
        }
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

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_State_Name"))
        {
            
            float yDifference = Mathf.Abs(target.transform.position.y - transform.position.y);

            Vector2 targetPosition;

            if (yDifference > verticalTolerance)
            {
                
                targetPosition = new Vector2(transform.position.x, target.transform.position.y);
            }
            else
            {
                
                targetPosition = new Vector2(target.transform.position.x, transform.position.y);
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        attackMode = true;
        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
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

        if (cooling)
        {
            anim.SetBool("Attack", false);
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
        timer = intTimer;
    }

    void Flip(float targetX)
    {
        if (targetX > transform.position.x && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(originalScaleMagnitudeX, transform.localScale.y, transform.localScale.z);
        }
        else if (targetX < transform.position.x && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-originalScaleMagnitudeX, transform.localScale.y, transform.localScale.z);
        }
    }

    void RaycastDebugger()
    {
        if (target == null) return;
        Vector2 direction = (target.transform.position.x < transform.position.x) ? Vector2.left : Vector2.right;

        if (distance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.green);
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
    }
}