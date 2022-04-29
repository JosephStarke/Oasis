using UnityEngine;

public class Dummy : EnemyStats
{    
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 10;
        this.attackDamage.BaseValue = 1;
        this.moveSpeed.BaseValue = 1;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.currentHealth = this.maxHealth.GetValue;
        this.knockbackPower.BaseValue = 10;

        //Enemy Specific Stats
        this.attackRange.BaseValue = 1f;
        this.attackSpeed.BaseValue = 1f;
        this.challengeRating.BaseValue = 1;
    }
}
