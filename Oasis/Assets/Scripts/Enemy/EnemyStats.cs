using Pathfinding;
using System.Collections;
using Jozzuph.EntityStats;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public Transform attackTarget;
    public AIDestinationSetter dest;
    public AIPath pather;
    public Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteBod;
    Color originalColorBod;
    [SerializeField] private GameObject body;
    [SerializeField] private Vector2 originalScaleBody;

    public PlayerStats player;
    public bool targetInRange = false;
    public bool canAttack = true;

    Animator animator;

    #region New Stats
    public Stat attackRange;
    public Stat attackSpeed;
    public Stat challengeRating;
    #endregion

    private void Awake()
    {
        dest = GetComponent<AIDestinationSetter>();
        pather = GetComponent<AIPath>();
        attackTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject; //get the body
        hitbox = body.transform.GetChild(0).gameObject.GetComponent<Collider2D>(); //get the hitbox
        animator = body.GetComponent<Animator>();
        originalScaleBody = body.transform.localScale;

        dest.target = attackTarget; //sets the desination target
        pather.maxSpeed = moveSpeed.GetValue; //sets the speed of the destination seeter to the move speed of the enemy

        player = attackTarget.gameObject.GetComponent<PlayerStats>(); //gets the PlayerStats and sets it as attack target
        spriteBod = transform.GetChild(0).GetComponent<SpriteRenderer>(); //This > body
        originalColorBod = spriteBod.color;
        blinkTime = 0.2f;
        initializeStats();
    }
    public override void Die()
    {
        this.attackRange.BaseValue = 0;
        pather.canMove = false;
        rb.isKinematic = true;
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        animator.SetBool("isDead", true); //play the animation
        Destroy(gameObject, 1); //destroy the object
        Debug.Log(transform.name + " died as an enemy ");
    }

    public override void SetBlinkColor()
    {
        spriteBod.color = desiredBlinkColor;
    }

    public override void SetOriginalColor()
    {
        spriteBod.color = originalColorBod;
    }

    #region Knockback / Take Damage
    public override void OnCollisionEnter2D(Collision2D otherEntity)
    {
        if (otherEntity.gameObject.CompareTag("PlayerWeapon") && hitbox == otherEntity.GetContact(0).otherCollider) //Check if the collided object is the player, and the player collided into the hitbox. This works because only the players urtbox collider collides with the enemies hitbox
        {
            EntityStats attackerStats = otherEntity.transform.root.gameObject.GetComponent<EntityStats>();

            //Deal Damage
            this.TakeDamage(attackerStats.attackDamage.GetValue, attackerStats.knockbackPower.GetValue, attackerStats.transform.position);
        }
    }

    public override IEnumerator Knockback(Vector2 knockback)
    {
        rb.isKinematic = false;

        Vector3 currentVelocity = pather.velocity;
        float recoverTime = pather.maxSpeed;

        //limiter variables
        Vector2 knockbackAbs = new Vector2(Mathf.Abs(knockback.x), Mathf.Abs(knockback.y));
        Vector2 currentVelocityAbs = new Vector2(Mathf.Abs(currentVelocity.x), Mathf.Abs(currentVelocity.y));
        int knockbackTicks = 0;
        int maxTicks = 5; //The more ticks the more long and smooth the knockback will be
        //Debug.Log("knockbackAbs: " + knockbackAbs);
        //Debug.Log("currentVelocityAbs: " + currentVelocityAbs);

        //Set max limits to prevent a large amount of ticks
        if (currentVelocityAbs.x < knockbackAbs.x / maxTicks)
        {
            currentVelocityAbs.x = knockbackAbs.x / maxTicks;
        }

        if (currentVelocityAbs.y < knockbackAbs.y / maxTicks)
        {
            currentVelocityAbs.y = knockbackAbs.y / maxTicks;

        }

        //Debug.Log("knockbackAbs Post: " + knockbackAbs);
        //Debug.Log("currentVelocityAbs Post: " + currentVelocityAbs);
        //calculate how many iterations to run through
        while (knockbackAbs != Vector2.zero) //calculate how many iterations to run through
        {
            knockbackAbs = new Vector2(knockbackAbs.x - currentVelocityAbs.x, knockbackAbs.y - currentVelocityAbs.y);

            if (knockbackAbs.x < 0)
            {
                knockbackAbs.x = 0;
            }

            if (knockbackAbs.y < 0)
            {
                knockbackAbs.y = 0;
            }
            knockbackTicks++;
        }
        //Debug.Log("knockbackTicks: " + knockbackTicks);

        pather.enabled = false; //disable pathing
        //Debug.Log("Pather Disabled");

        //Debug.Log("Original Knockback: " + knockback);
        Vector2 knockbackSub = new Vector2((knockback.x / knockbackTicks), (knockback.y / knockbackTicks)); //how many iterations tho slow down the knockback
        //Debug.Log("knockbackSub: " + knockbackSub);
        //apply knockback and knockback slowdown
        for (int i = 0; i <= knockbackTicks; i++)
        {
            rb.velocity = knockback; //set targets velocity
            //Debug.Log("Velocity Pre: " + rb.velocity);
            knockback = new Vector2(knockback.x - knockbackSub.x, knockback.y - knockbackSub.y); //update knockback value 
            //Debug.Log("Velocity Post: " + rb.velocity);
            yield return new WaitForSeconds(1 / (Mathf.Max(Mathf.Pow(recoverTime, 2), 10))); //the higher speed the target has the faster the target will recover (exponentially) speed * 10

            if (rb == null)
            {
                pather.enabled = true;
                break;
            }

            if (rb.velocity == Vector2.zero)
            {
                break;
            }
        }

        if (rb != null)
        {
            pather.enabled = true; //enable pathing
            //Debug.Log("Pather Enabled");
        }
    }
    #endregion

    #region Enemy Functions
    public void StopOnRange()
    {
        if (canAttack == true)
        {
            if (Vector3.Distance(gameObject.transform.position, dest.target.transform.position) <= attackRange.GetValue)
            {
                pather.maxSpeed = 0;
            }
            else
            {
                pather.maxSpeed = moveSpeed.GetValue;
            }
        }
    }

    public void Attack()
    {
        if (attackTarget != null)
        {

            CheckTargetInRange();
            if (targetInRange && canAttack)
            {
                //Animation
                FaceTarget();
                animator.SetBool("isAttacking", true);

                //make it wait on a cooldown
                StartCoroutine(AttackCooldown());
                //check if PlayerStats is dead if they are target nothing
            }
            else
            {
                animator.SetBool("isAttacking", false);
            }
        }
    }

    private void CheckTargetInRange()
    {
        if (Vector3.Distance(attackTarget.position, transform.position) <= attackRange.GetValue)
        {
            targetInRange = true;
        }
        else if (Vector3.Distance(attackTarget.position, transform.position) > attackRange.GetValue)
        {
            targetInRange = false;
        }
    }
    private IEnumerator AttackCooldown()
    {
        pather.maxSpeed = 0;

        if (rb != null)
        {
            rb.isKinematic = true; //disable physics so their face doesn't push them
        }

        canAttack = false;
        yield return new WaitForSeconds(1 / this.attackSpeed.GetValue);
        canAttack = true;

        if (rb != null)
        {
            rb.isKinematic = false; //enable physics
        }
        pather.maxSpeed = moveSpeed.GetValue;
    }

    public void ChangeDirection()
    {
        if (pather.velocity.x < 0) //if moving rights
        {
            body.transform.localScale = originalScaleBody; //turn left
        }
        else if (pather.velocity.x > 0) //if moving left
        {
            body.transform.localScale = new Vector2(-originalScaleBody.x, originalScaleBody.y); //turn right
        }
    }

    public void FaceTarget()
    {
        Vector3 direction = -(this.transform.position - player.transform.position).normalized; //get direction of target

        if (direction.x < 0)
        {
            body.transform.localScale = originalScaleBody; //turn left
        }
        else if (direction.x > 0) //if moving left
        {
            body.transform.localScale = new Vector2(-originalScaleBody.x, originalScaleBody.y); //turn right
        }
    }

    public void OnDrawGizmos()
    {
        if (transform == null)
        {
            return;
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange.GetValue);
    }
    #endregion
}
