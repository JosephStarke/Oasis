using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap worldMap;
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
            if (dataFromAllTiles[worldMap.GetTile(position)].isHealthy == true)
            {
                availableTileList.Add(position);
                Debug.Log("Positions: " + position + " Added to healthy Tiles");
            }
        }

        return availableTileList;
    }

    public Vector2 chooseTile(List<Vector3Int> tileList)
    {
        Vector2 chosenTileCenter;

        Debug.Log("Amount of tiles: " + tileList.Count);
        //choose tile index
        Vector3Int chosenTilePos = tileList[Random.Range(0, tileList.Count)]; //-1?
        chosenTileCenter = GetAnchoredTileCenter(chosenTilePos);

        Debug.Log("Tile center chosen: " + chosenTileCenter);

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

    public Vector2 GeneratePointInTile(Vector2 point)
    {
        int coin; 

        float translateX;
        float translateY;

        //Generate multipliers
        translateX = Random.Range(0f, (distanceToEdge * (gridScale.x/1.5f)));
        translateY = Random.Range(0f, distanceToEdge * (gridScale.y/1.5f));

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
}
