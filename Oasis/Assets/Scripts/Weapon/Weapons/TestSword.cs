using UnityEngine;

public class TestSword : WeaponStats
{
    public override void initializeStats()
    {
        #region Effects Player
        this.maxHealth.BaseValue = 0;
        this.attackDamage.BaseValue = 1;
        this.moveSpeed.BaseValue = 0;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 10;
        #endregion

        #region Effects Weapon
        this.maxDurability.BaseValue = 999;
        this.currentDurability = maxDurability.GetValue;
        #endregion
    }
}
