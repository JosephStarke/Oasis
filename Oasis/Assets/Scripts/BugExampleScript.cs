using UnityEngine;
using UnityEngine.Tilemaps;

public class BugExampleScript : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject player;
    private Vector2 mousePosition;
    private Vector2 anchorOffset;
    private Vector3Int chosenTile;
    private Vector2 chosenTileCenter;

    [SerializeField] private Vector3 tilemapScale;

    private void Awake()
    {
        anchorOffset = tilemap.GetComponent<Tilemap>().tileAnchor; //Grabs the anchor vector

        tilemapScale = transform.localScale;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse pos on screen
            chosenTile = tilemap.WorldToCell(mousePosition); //get the tile at the mouse position

            chosenTileCenter = tilemap.GetCellCenterWorld(chosenTile); //get the center of the chosentile WITHOUT the anchor offset
            
            chosenTileCenter -= (anchorOffset * (tilemapScale.y * 2)); //apply anchor offset to the tile

            /*if (chosenTileCenter.y > 1) //This should be the row above the - y coordinate point
            {
                Debug.LogError("chosenTile: " + tilemap.WorldToCell(chosenTileCenter).y % 2);
                if (tilemap.WorldToCell(chosenTileCenter).y % 2 == 1)
                {
                    chosenTileCenter.x += tilemapScale.x * 1.28f;
                }
                else
                {
                    chosenTileCenter.x -= tilemapScale.x * 1.28f;
                }
            }*/

            Debug.LogError("chosenTileCenter: " + chosenTileCenter);
            player.transform.position = chosenTileCenter; //use this to test through object teleporting to center
        }
    }
}
