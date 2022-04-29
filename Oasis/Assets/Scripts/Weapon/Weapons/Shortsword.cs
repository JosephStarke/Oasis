using UnityEngine;

public class Shortsword : WeaponStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 999;
        this.attackDamage.BaseValue = Random.Range(1, 3);
        this.moveSpeed.BaseValue = 0;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 0;
    }
}
