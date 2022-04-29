using Jozzuph.EntityStats;
using System.Collections;
using UnityEngine;


public class EquipableStats : EntityStats
{
    public Stat maxDurability;
    public float currentDurability;

    public override void initializeStats()
    {
        base.initializeStats();
        this.maxDurability.BaseValue = 0;
        this.currentDurability = this.maxDurability.GetValue;
    }

    public override void TakeDamage(float damage)
    {
        currentDurability -= damage;

        //check death
        if (currentDurability <= 0)
        {
            currentDurability = 0;
            Die();
        }
    }
}
