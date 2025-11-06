using UnityEngine;

public class MeleeSkeletonBehavior : MonoBehaviour
{
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;

    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    private float intTimer;
    private Vector2 targetDirection;

    void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
        targetDirection = Vector2.right;
    }

    void Update()
    {
 
        if (cooling)
        {
            Cooldown();
        }

        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, targetDirection, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        if (inRange && hit.collider != null)
        {
            EnemyLogic();
        }
        else if (inRange && hit.collider == null)
        {
           
        }

        if (inRange == false)
        {
            anim.SetBool("isWalking", false);
            StopAttack();
            target = null;
        }
    }


    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            target = trig.gameObject;
            inRange = true;
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if (distance <= attackDistance && cooling == false)
        {
            Attack();
        }

    }


    void Move()
    {
        anim.SetBool("isWalking", true);

        Flip();

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Flip()
    {
        if (target != null)
        {
            if (target.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                targetDirection = Vector2.right;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                targetDirection = Vector2.left;
            }
        }
    }

    void Attack()
    {
        if (cooling) return;

        timer = intTimer;
        cooling = true;
        attackMode = true;
        anim.SetBool("isWalking", false);
        anim.SetBool("Attack", true);
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            cooling = false;
        }


    }


    void StopAttack()
    {
        attackMode = false;
        anim.SetBool("Attack", false);
    }



    void RaycastDebugger()
    {
        Color color = (distance > attackDistance) ? Color.red : Color.green;
        Debug.DrawRay(rayCast.position, targetDirection * rayCastLength, color);
    }

}