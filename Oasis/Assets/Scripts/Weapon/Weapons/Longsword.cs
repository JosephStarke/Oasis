using UnityEngine;

public class Longsword : WeaponStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 999;
        this.attackDamage.BaseValue = Random.Range(2, 5); ;
        this.moveSpeed.BaseValue = 0;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 0;
    }
}
