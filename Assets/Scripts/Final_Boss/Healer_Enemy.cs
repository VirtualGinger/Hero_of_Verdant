using UnityEngine;

public class Healer_Enemy : MonoBehaviour
{
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float healDistance;
    public float moveSpeed;
    public float timer;
    public float verticalTolerance = 0.5f;
    public bool useDiagonalMovement = false;

    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool healMode;
    private bool inRange;
    private bool cooling;
    private float intTimer;
    private bool isFacingRight = true;
    private float originalScaleMagnitudeX;
    private EnemyHealth _enemyHealth;

    void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
        originalScaleMagnitudeX = Mathf.Abs(transform.localScale.x);
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (_enemyHealth.health <= 0)
        {
            _enemyHealth.health = 0;
            anim.SetBool("canWalk", false);
            anim.SetBool("Attack", false);
            return;
        }

        FindClosestAlly();

        if (target != null)
        {
            float targetX = target.transform.position.x;
            Flip(targetX);

            Vector2 direction = (targetX < transform.position.x) ? Vector2.left : Vector2.right;
            hit = Physics2D.Raycast(rayCast.position, direction, rayCastLength, raycastMask);

            RaycastDebugger();

            distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance > healDistance)
            {
                Move();
                StopHeal();
            }
            else if (distance <= healDistance && cooling == false)
            {
                Heal();
            }
        }
        else
        {
            anim.SetBool("canWalk", false);
            StopHeal();
        }

        if (cooling)
        {
            Cooldown();
        }
    }

    // ----- FIND CLOSEST OTHER ENEMY -----
    void FindClosestAlly()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float bestDist = Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (GameObject e in enemies)
        {
            if (e == this.gameObject) continue;

            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                bestTarget = e;
            }
        }

        target = bestTarget;
    }

    void Move()
    {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_State_Name"))
        {
            if (useDiagonalMovement)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position,
                    moveSpeed * Time.deltaTime);
                return;
            }

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

            transform.position = Vector2.MoveTowards(transform.position, targetPosition,
                moveSpeed * Time.deltaTime);
        }
    }

    void Heal()
    {
        healMode = true;
        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);

        EnemyHealth allyHP = target.GetComponent<EnemyHealth>();
        if (allyHP != null)
        {
            allyHP.health += 1;
        }
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            cooling = false;
            healMode = false;
            timer = intTimer;
        }

        if (cooling)
        {
            anim.SetBool("Attack", false);
        }
    }

    void StopHeal()
    {
        cooling = false;
        healMode = false;
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

        if (distance > healDistance)
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.red);
        else
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.green);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    public void ResetWeaponHitbox()
    {
        Damage weaponDamage = GetComponentInChildren<Damage>();

        if (weaponDamage != null)
        {
            weaponDamage.hasHit = false;
        }
    }
}