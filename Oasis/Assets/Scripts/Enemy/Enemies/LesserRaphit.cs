public class LesserRaphit : EnemyStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 1;
        this.attackDamage.BaseValue = 1;
        this.moveSpeed.BaseValue = 1;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.currentHealth = this.maxHealth.GetValue;
        this.knockbackPower.BaseValue = 10f;

        //Enemy Specific Stats
        this.attackRange.BaseValue = 1.0f;
        this.attackSpeed.BaseValue = 1f;
        this.challengeRating.BaseValue = 1f;
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
