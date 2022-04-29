public class Dagger : WeaponStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 10;
        this.attackDamage.BaseValue = 1;
        this.moveSpeed.BaseValue = 0;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 0;
    }
}
