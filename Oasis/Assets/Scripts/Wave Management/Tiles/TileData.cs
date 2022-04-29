using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public bool isHealthy;
    public bool isPassable;
    public float speedModifer = 1f;
}
