using UnityEngine;
public class PlayerKnockback : MonoBehaviour
{
    //GOES ON BODY
    [SerializeField] private PlayerStats player;
    private void Awake()
    {
        player = transform.root.GetComponent<PlayerStats>();
    }

    public void OnCollisionEnter2D(Collision2D otherEntity)
    {
        if (otherEntity.gameObject.CompareTag("Enemy") && player.hitbox == otherEntity.GetContact(0).otherCollider) //Check if the collided object is the player, and the player collided into the hitbox. This works because only the players urtbox collider collides with the enemies hitbox
        {
            EntityStats attackerStats = otherEntity.transform.root.gameObject.GetComponent<EntityStats>();

            //Take Damage
            player.TakeDamage(attackerStats.attackDamage.GetValue, attackerStats.knockbackPower.GetValue, attackerStats.transform.position);
        }
    }
}
