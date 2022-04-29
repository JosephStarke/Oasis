using Pathfinding;
using System.Collections;
using UnityEngine;

public class WeaponStats : EquipableStats
{
    private void Awake()
    {
        //instantiate variables
        //hitBox = GetComponent<BoxCollider2D>();
        initializeStats();
    }

    public override void Die()
    {
        Destroy(gameObject);
        Debug.Log(transform.name + " broke");
    }
}
