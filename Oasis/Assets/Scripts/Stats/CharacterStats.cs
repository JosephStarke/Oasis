using System.Collections;
using UnityEngine;

public class CharacterStats : EntityStats
{
    #region New Stats
    public float currentHealth; //{ get; private set; } //any class can get but can only be set in this class
    #endregion

    #region Damage Blinking Variables
    public Coroutine damagedBlink;
    public float blinkTime;
    public Color desiredBlinkColor = new Color((255 / 255), (89 / 255), (94 / 255)); //colors are normalized from 0 - 1 so this needs to be done
    #endregion

    public Collider2D hitbox;
    public bool previouslyInitialized = false;

    public override void initializeStats()
    {
        base.initializeStats();
        currentHealth = this.maxHealth.GetValue;
    }
    
    #region Take Damage Characters
    public override void TakeDamage(float damage) //float attackerKnockback
    {
        #region Apply Agility
        if (agility.GetValue != 0)
        {
            if (AttemptDodge() == true) //if the Attempt dodge function returns true
            {
                damage = 0;
                return;
            }
        }
        #endregion
  
        #region Apply Armor
        if (armor.GetValue != 0)
        {
            damage *= (100 / (100 + armor.GetValue)); //league of legends armor equation (100 armor reduces damage by 50%)
        }
        else
        {
            damage *= 2 - (100 / (100 - armor.GetValue)); //should come out to 1
        }

        damage = Mathf.Clamp(damage, 0, int.MaxValue); //prevent from healing by blocking damage
        #endregion

        //deal damage
        BlinkHurt();
        currentHealth -= damage;

        //check death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    #region Knockback
    public override void TakeDamage(float damage, float attackerKnockback, Vector3 attackerPos)
    {
        TakeDamage(damage); //normal take damage function

        if (currentHealth > 0) //if still alive try knockback
        {
            if (attackerKnockback != 0) //if less than 0 it should pull
            {
                Vector3 knockDirection = Vector3.zero;
                Vector2 knockVelocity = Vector2.zero;

                if (this.GetComponent<Rigidbody2D>() != null)
                {
                    //Knockback

                    knockDirection = (this.transform.position - attackerPos).normalized;
                    //Debug.LogError("knockDirection: " + knockDirection);

                    knockVelocity = knockDirection * attackerKnockback; //Get the correct amount of knockback
                    //Debug.LogError("knockVelocity: " + knockVelocity);

                    //Debug.DrawLine(transform.position, (transform.position + (knockDirection * attackerKnockback)), Color.red, 5f); //show line for knocback

                    if (routineKnockback != null)
                    {
                        StopCoroutine(routineKnockback); //if the object already has knockback applied this will stop that knockback
                    }
                    routineKnockback = StartCoroutine(Knockback(knockVelocity)); //apply newest knockback and set it to routinKnockback
                }
            }
        }
    }
    #endregion

    private bool AttemptDodge()
    {
        bool dodged;

        float dodgeThreshold = (100 / (100 + agility.GetValue));
        dodgeThreshold = 1 - dodgeThreshold;
        float dodgeChance = Random.Range(0f, 1f);

        if (dodgeChance <= dodgeThreshold)
        {
            dodged = true;
            Debug.Log(transform.name + " dodged = " + dodged);
        }
        else
        {
            dodged = false;
        }

        return dodged;
    }
    #endregion

    #region Blink Functions
    public virtual void BlinkHurt()
    {
        if (damagedBlink != null)
        {
            SetOriginalColor(); //make sure the entity isnt still the blink color beofre turning back
            StopCoroutine(damagedBlink); //if the object already has knockback applied this will stop that knockback
        }
        damagedBlink = StartCoroutine(DamagedBlink());
    }
    public virtual void SetOriginalColor() {}
    public virtual void SetBlinkColor() {}

    public IEnumerator DamagedBlink()
    {
        SetBlinkColor();
        yield return new WaitForSeconds(blinkTime); //the actual wait time
        SetOriginalColor();
    }
    #endregion
}
