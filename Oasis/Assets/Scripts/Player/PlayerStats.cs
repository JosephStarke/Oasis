using System.Collections;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public GameObject enemy;
    public PlayerMovement playerMovement;
    [SerializeField] private GameObject currentWeapon;
    [SerializeField] private GameObject previousWeapon;
    [SerializeField] private GameObject fist;
    private BoxCollider2D colFist;
    [SerializeField] public WeaponStats weaponStats;

    [SerializeField] private SpriteRenderer spriteBod;
    [SerializeField] private SpriteRenderer spriteFist;
    private Color originalColorBod;
    private Color originalColorFist;

    private bool changedWeapon = false;

    public Animator animator;


    private void Awake()
    {
        //instantiate variables
        //weapon = transform.GetChild(1).GetChild(1).gameObject; //This > weapon pivot > weapon gameobject
        playerMovement = GetComponent<PlayerMovement>();
        animator = transform.GetChild(0).GetComponent<Animator>(); //This > body
        fist = transform.GetChild(1).GetChild(0).gameObject;
        colFist = fist.GetComponent<BoxCollider2D>();
        weaponStats = fist.GetComponent<WeaponStats>();
        spriteBod = transform.GetChild(0).GetComponent<SpriteRenderer>(); //This > body
        spriteFist = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>(); //This > weaponpivot > fist

        originalColorBod = spriteBod.color;
        originalColorFist = spriteFist.color;

        hitbox = spriteBod.transform.GetChild(0).gameObject.GetComponent<Collider2D>(); //get the hitbox
        initializeStats();
        blinkTime = 0.2f;
    }
    private void Update()
    {
        currentWeapon = CheckForWeapon(); //Check for weapon wether it is fist or something in slot 1

        if (previousWeapon != currentWeapon)
        {
            changedWeapon = true;
            previousWeapon = currentWeapon;
        }
        if (changedWeapon)
        {
            weaponStats = currentWeapon.GetComponent<WeaponStats>();
            updateStats();
            changedWeapon = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            this.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Instantiate(enemy, enemy.transform);
        }
    }

    #region Stat Functions
    public override void Die()
    {
        animator.SetBool("isDead", true);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(false); //remove PlayerStats > armpivot > arm
        if (transform.GetChild(1).childCount > 1)
        {
            transform.GetChild(1).GetChild(1).gameObject.SetActive(false); //remove PlayerStats > armpivot > weapon
        }
        Debug.Log(transform.name + " died ");
        this.GetComponent<PlayerMovement>().enabled = false; //remove movement script so PlayerStats cant move
        this.GetComponent<Rigidbody2D>().isKinematic = true; //disables physics
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero; //disables remove all speed
    }

    public override void BlinkHurt()
    {
        if (damagedBlink != null)
        {
            SetOriginalColor(); //make sure the entity isnt still the blink color beofre turning back
            StopCoroutine(damagedBlink); //if the object already has knockback applied this will stop that knockback
        }
        damagedBlink = StartCoroutine(DamagedBlink());
    }

    public override void SetBlinkColor()
    {
        spriteBod.color = desiredBlinkColor;
        spriteFist.color = desiredBlinkColor;
    }

    public override void SetOriginalColor()
    {
        spriteBod.color = originalColorBod;
        spriteFist.color = originalColorFist;
    }

    #region Apply Knockback

    public override IEnumerator Knockback(Vector2 knockback)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector3 currentVelocity = rb.velocity;
        float recoverTime = moveSpeed.GetValue;

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


        playerMovement.isKnocked = true; //stops player movement
        //playerMovement.enabled = false; //disable pathing
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
                break;
            }

            if (rb.velocity == Vector2.zero)
            {
                break;
            }
        }

        if (rb != null)
        {
            playerMovement.isKnocked = false; //allows player movement
            //playerMovement.enabled = true; //enable pathing
            //Debug.Log("Pather Enabled");
        }
    }
    #endregion
    #endregion

    #region Additional Functions
    private GameObject CheckForWeapon()
    {
        GameObject currentWeapon;
        if (transform.GetChild(1).childCount > 1)
        {
            currentWeapon = transform.GetChild(1).GetChild(1).gameObject;
            colFist.enabled = false;
        }
        else
        {
            currentWeapon = fist;
            colFist.enabled = true;
        }

        return currentWeapon;
    }

    private void updateStats()
    {
        initializeStats(); //reset stats to no weapon
        //initialize new stats
        AddStat(this.maxHealth, weaponStats.maxHealth.GetValue);
        AddStat(this.attackDamage, weaponStats.attackDamage.GetValue);
        AddStat(this.moveSpeed, weaponStats.moveSpeed.GetValue);
        AddStat(this.armor, weaponStats.armor.GetValue);
        AddStat(this.agility, weaponStats.agility.GetValue);
        AddStat(this.knockbackPower, weaponStats.knockbackPower.GetValue);

        this.currentHealth += weaponStats.maxHealth.GetValue;
    }
    #endregion
}
