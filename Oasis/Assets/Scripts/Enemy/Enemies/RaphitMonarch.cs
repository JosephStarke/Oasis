public class RaphitMonarch : EnemyStats
{
    public override void initializeStats()
    {
        this.maxHealth.BaseValue = 10;
        this.attackDamage.BaseValue = 3;
        this.moveSpeed.BaseValue = 3;
        this.armor.BaseValue = 0;
        this.agility.BaseValue = 0;
        this.currentHealth = this.maxHealth.GetValue;
        this.knockbackPower.BaseValue = 5f;

        //Enemy Specific Stats
        this.attackRange.BaseValue = 3f;
        this.attackSpeed.BaseValue = 1f;
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