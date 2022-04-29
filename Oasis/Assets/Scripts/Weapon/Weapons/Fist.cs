public class Fist : WeaponStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 999999;
        this.attackDamage.BaseValue = 0.5f;
        this.moveSpeed.BaseValue = 0;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 0;

        this.maxDurability.BaseValue = 999;
        this.currentDurability = maxDurability.GetValue;
    }
}
