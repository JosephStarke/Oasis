public class RaphitCharger : EnemyStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 5;
        this.attackDamage.BaseValue = 2;
        this.moveSpeed.BaseValue = 4;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 10;
        this.currentHealth = this.maxHealth.GetValue;
        this.knockbackPower.BaseValue = 15f;

        //Enemy Specific Stats
        this.attackRange.BaseValue = 1.0f;
        this.attackSpeed.BaseValue = 2.5f;
        this.challengeRating.BaseValue = 5f;
    }

    private void Update()
    {
        if (player.currentHealth > 0)
        {
            Attack();
            StopOnRange();
        }
        else
        {
            dest.target = null;
        }
        ChangeDirection();
    }
}
