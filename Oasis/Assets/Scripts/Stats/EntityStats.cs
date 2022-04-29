using UnityEngine;
using Jozzuph.EntityStats;
using System.Collections;

public class EntityStats : MonoBehaviour
{
    public Coroutine routineKnockback;

    #region All Stats
    public Stat maxHealth;
    public Stat attackDamage;
    public Stat moveSpeed;
    public Stat armor;
    public Stat agility;
    public Stat knockbackPower;
    //public Stat attackTime;
    #endregion

    private void Awake()
    {
        initializeStats();
    }
    public virtual void initializeStats() //virtual helps with overriding
    {
        this.maxHealth.BaseValue = 0;
        this.attackDamage.BaseValue = 0;
        this.moveSpeed.BaseValue = 0;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 0;
    }
    public virtual void TakeDamage(float damage) { }
    public virtual void TakeDamage(float damage, float attackerKnockback, Vector3 attackerPos) { }

    public virtual void OnCollisionEnter2D(Collision2D otherCollider) { }

    public virtual IEnumerator Knockback(Vector2 knockback)
    {
        yield return 0;
    }

    public virtual void Die()
    {
        //Die in some way
        //This is meant to be overidden
        Debug.Log(transform.name + " died ");
    }
    public void AddStat(Stat stat, float change)
    {
        //player.maxHealth.AddModifier(new StatModifier(1, StatModType.PercentAdd, this));
        stat.AddModifier(new StatModifier(change, StatModType.Flat, this));
    }

    public void AddPercentStat(Stat stat, float change)
    {
        //player.maxHealth.AddModifier(new StatModifier(1, StatModType.PercentAdd, this));
        stat.AddModifier(new StatModifier(change, StatModType.PercentAdd, this));
    }

    public void MultPercentStat(Stat stat, float change)
    {
        //player.maxHealth.AddModifier(new StatModifier(1, StatModType.PercentAdd, this));
        stat.AddModifier(new StatModifier(change, StatModType.PercentMult, this));
    }
}
