using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] Vector2 center, A, B, C, D, E, F;
    [SerializeField] private Tilemap worldMap;
    [SerializeField] private Tilemap obstacleMap;
    //private EdgeCollider2D edgeCollider;

    [SerializeField] private List<TileData> tileDatas;
    [SerializeField] private Vector2 anchorOffset;
    [SerializeField] private Vector2 gridScale;
    [SerializeField] private float xTranslation;

    public float distanceToEdge = 1.28f;

    //public Vector2 edgePoint; for debugging

    //[SerializeField] private List<Vector3> availableTileList;

    private Dictionary<TileBase, TileData> dataFromAllTiles;

    public TileBase clickedTile;
    private void Awake()
    {
        worldMap = transform.GetChild(0).GetComponent<Tilemap>();
        obstacleMap = transform.GetChild(1).GetComponent<Tilemap>();
        //edgeCollider = obstacleMap.GetComponent<EdgeCollider2D>();

        UpdateTileData();
        gridScale = transform.localScale;
        anchorOffset = GameObject.Find("GridManager").transform.GetChild(0).GetComponent<Tilemap>().tileAnchor; //Gridmanger > Tilemap > Tilemap > Anchorpoints
        anchorOffset *= (gridScale * 2); //Calculate anchoroffset wih the grid scale
        xTranslation = (gridScale.x * distanceToEdge); //find the center for x coord
    }

    public void UpdateTileData()
    {
        dataFromAllTiles = new Dictionary<TileBase, TileData>(); //reset tilemap info

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromAllTiles.Add(tile, tileData);
            } //grab specific data for each of thos tiles
        } //grab the tile
    }

    public List<Vector3Int> FindAvailableTiles()
    {
        List<Vector3Int> availableTileList = new List<Vector3Int>();

        foreach (var position in worldMap.cellBounds.allPositionsWithin) //all posiitons on the tilemap
        {
            if (!worldMap.HasTile(position))
            {
                continue;
            } //skip if no tile exists

            //check for available tiles, if it is healthy it will be added to the list
            if (dataFromAllTiles[worldMap.GetTile(position)].isHealthy == true && dataFromAllTiles[worldMap.GetTile(position)].isInfected == false) //Checks if its a healthy not infected tile
            {
                availableTileList.Add(position);
                Debug.Log("Positions: " + position + " Added to Avialable Tiles");
            }
        }

        return availableTileList;
    }

    public Vector2 ChooseTile(List<Vector3Int> tileList)
    {
        Vector2 chosenTileCenter;

        Debug.Log("Amount of tiles: " + tileList.Count);
        //choose tile index
        Vector3Int chosenTilePos = tileList[Random.Range(0, tileList.Count)];
        chosenTileCenter = GetAnchoredTileCenter(chosenTilePos);

        Debug.Log("Tile center chosen: " + chosenTileCenter);

        //infect tile
        //InfectTile(chosenTilePos);

        return chosenTileCenter;
    }

    private Vector2 GetAnchoredTileCenter(Vector3Int tile)
    {
        Vector2 tileCenter;
        tileCenter = worldMap.GetCellCenterWorld(tile); //get the center of the chosentile WITHOUT the anchor offset
        tileCenter -= anchorOffset; //apply anchor offset to the tile


        if (tileCenter.y > 1) //if the tiles are on the upper half
        {
            if (worldMap.WorldToCell(tileCenter).y % 2 == 1)
            {
                tileCenter.x += xTranslation;
            }
            else
            {
                tileCenter.x -= xTranslation;
            }
        }

        //edgePoint = new Vector2(tileCenter.x + (distanceToEdge * (gridScale.y / 1.5f)), tileCenter.y + (distanceToEdge * (gridScale.y / 1.5f)));

        return tileCenter;
    }

    private void InfectTile(Vector3Int tile)
    {
        if (dataFromAllTiles[worldMap.GetTile(tile)].isHealthy == true && dataFromAllTiles[worldMap.GetTile(tile)].isInfected == false) //LOOK up unity how to store specific tile data instead of all tiles of that type
        {
            dataFromAllTiles[worldMap.GetTile(tile)].isInfected = true;
            Debug.Log("Tile: " + tile + " is now infected...");
        }
        else if (dataFromAllTiles[worldMap.GetTile(tile)].isInfected == true)
        {
            Debug.LogError("Tile: " + tile + " was selected but it is already infected...");
        }
        else
        {
            Debug.LogError("Tile: " + tile + " was selected and is not a healthy tile...");
        }
    }

    public Vector2 GeneratePointInTile(Vector2 point)
    {
        int coin; 

        float translateX;
        float translateY;

        //Generate multipliers
        translateX = Random.Range(0f, (distanceToEdge * (gridScale.x/1.6f)));
        translateY = Random.Range(0f, distanceToEdge * (gridScale.y/1.6f));

        //Genterate + or - (Coinflip)
        //X
        coin = Random.Range(0, 2);
        //Debug.Log("Flip X: " + coin);
        if (coin == 0) //subtract
        {
            point.x += translateX;
        }
        else //add
        {
            point.x -= translateX;
        }

        //Y
        coin = Random.Range(0, 2);
        //Debug.Log("Flip Y: " + coin);
        if (coin == 0) //subtract
        {
            point.y += translateY;
        }
        else //add
        {
            point.y -= translateY;
        }

        return point;
    }

    public bool CheckIfSameTile(Vector2 pos1, Vector2 pos2)
    {
        Vector3Int pos1Tile = worldMap.WorldToCell(pos1);
        Vector3Int pos2Tile = worldMap.WorldToCell(pos2);

        if (pos1Tile == pos2Tile)
        {
            return true;
        }
        return false;
    }

    public void GenerateTileWalls(Vector2 spawnerPos)
    {
        //Vector2 center, A, B, C, D, E, F;
        Vector3Int tileCenter = worldMap.WorldToCell(spawnerPos);
        
        center = GetAnchoredTileCenter(tileCenter);

        //Get verticies
        A = center + (new Vector2(0, distanceToEdge) * gridScale);
        B = center + (new Vector2(distanceToEdge, distanceToEdge/2) * gridScale);
        C = center + (new Vector2(distanceToEdge, -distanceToEdge / 2) * gridScale);

        D = center + (new Vector2(0, -distanceToEdge) * gridScale);
        E = center + (new Vector2(-distanceToEdge, -distanceToEdge / 2) * gridScale);
        F = center + (new Vector2(-distanceToEdge, distanceToEdge / 2) * gridScale);

        //Get Verticies edges
        EdgeCollider2D edgeCollider;
        edgeCollider = obstacleMap.gameObject.AddComponent<EdgeCollider2D>();

        Vector2[] colliderpoints = new Vector2[7];

        colliderpoints[0] = A;
        colliderpoints[1] = B;
        colliderpoints[2] = C;
        colliderpoints[3] = D;
        colliderpoints[4] = E;
        colliderpoints[5] = F;
        colliderpoints[6] = A;

        //make the points work correctly
        for (int i = 0; i < colliderpoints.Length; i++)
        {
            colliderpoints[i] /= gridScale;
        }
        edgeCollider.points = colliderpoints;
        edgeCollider.edgeRadius = 0.25f;
    }

    public void DestroyTileWalls()
    {
        Destroy(obstacleMap.gameObject.GetComponent<EdgeCollider2D>());
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(center, A);
        Gizmos.DrawLine(center, B);
        Gizmos.DrawLine(center, C);
        Gizmos.DrawLine(center, D);
        Gizmos.DrawLine(center, E);
        Gizmos.DrawLine(center, F);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(A, B);
        Gizmos.DrawLine(B, C);
        Gizmos.DrawLine(C, D);
        Gizmos.DrawLine(D, E);
        Gizmos.DrawLine(E, F);
        Gizmos.DrawLine(F, A);
    }*/
}
