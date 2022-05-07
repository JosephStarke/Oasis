using UnityEngine;

public class PlairBase : PlayerStats
{
    // Start is called before the first frame update
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 10;
        this.attackDamage.BaseValue = 0;
        this.moveSpeed.BaseValue = 3;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.knockbackPower.BaseValue = 0;

        if (!previouslyInitialized)
        {
            this.currentHealth = this.maxHealth.GetValue;
            previouslyInitialized = true;
        } //will not reset players current health in future initializations
    }
}
