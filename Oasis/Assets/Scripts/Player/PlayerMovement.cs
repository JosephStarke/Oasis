using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region Movement Variables
    public Rigidbody2D rb;
    [SerializeField] private GameObject body;
    [SerializeField] private Vector2 originalScaleBody;
    private Vector2 moveDirection; //Uses move x and y together
    private Coroutine forcedMove;
    private bool forcingMovement;

    Vector3 mousePos;
    private Vector3 targetPosition;
    //  private Vector2 targetPostion2D;

    public bool isKnocked = false;
    #endregion

    #region Tile Highlight Variables
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap tilemapObstacle;
    [SerializeField] Tilemap tilemapHighlight;
    [SerializeField] Tile highlightTile;

    private Vector3Int currentCellPos, previousCellPos;
    #endregion

    public PlayerStats PlayerStats;
    public Animator animator;

    public Camera playerCamera;

    private void Awake()
    {
        PlayerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>(); //Sets rb to the players rigid body
        body = transform.GetChild(0).gameObject; //get the body
        animator = body.GetComponent<Animator>();
        forcingMovement = false;

        originalScaleBody = body.transform.localScale;
    }

    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        if (!forcingMovement)
        {
            Move(); //Physics Calculations better on Fixed
        }
    }

    // do late so that the PlayerStats has a chance to move in update if necessary
    private void LateUpdate()
    {
        //TileManager();
    }

    #region Movement Functions
    void ProcessInputs()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get current mouse position
        mousePos.z = transform.position.z; //set current mouse position to the same z value as the transform this script is on

        if (Input.GetMouseButtonDown(1))
        {
            //ProcessTileJump();
        }
        if (moveDirection.x > 0) //if moving right
        {
            body.transform.localScale = originalScaleBody; //PlayerStats turn right
        }
        else if (moveDirection.x < 0) //if moving left
        {
            body.transform.localScale = new Vector2 (-originalScaleBody.x, originalScaleBody.y); //PlayerStats turn left 
        }
    }

    public void Move()
    {
        if (!isKnocked) //prevents player from moving while being knockedback
        {
            //Movement
            moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized; //normalized caps vector at 1
            rb.velocity = new Vector2(moveDirection.x * PlayerStats.moveSpeed.BaseValue, moveDirection.y * PlayerStats.moveSpeed.BaseValue); //Movement formula
        }
       
        //Animation
        if(rb.velocity.x != 0 || rb.velocity.y != 0) //run
        {
            animator.SetBool("isMoving", true);
        }
        else //idle
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void ForceMove(float time)
    { 
        if (forcedMove != null)
        {
            StopCoroutine(ForcedMove(time));
        }

        forcedMove = StartCoroutine(ForcedMove(time));
    }

    private IEnumerator ForcedMove(float time)
    {
        forcingMovement = true;
        Debug.Log("Forcing player movement for " + time + " seconds...");
        yield return new WaitForSeconds(time);
        Debug.Log("No longer forcing player movement...");
        forcingMovement = false;
    }

    /*private void Collide()
    {
        rb.velocity = -rb.velocity; //Move PlayerStats back
        moveDirection = new Vector2(0, 0); //reset direction
    }*/

    /*private void ProcessTileJump()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, targetPosition); //rotate towards direction
        transform.position = tilemapHighlight.GetCellCenterWorld(currentCellPos); //teleport to cell center
    }
    #endregion

    #region Tile Highlight Functions
    private void TileManager()
    {
        currentCellPos = tilemapHighlight.WorldToCell(mousePos); // get current grid location, at the mouses location

        if (currentCellPos != previousCellPos) // if the position has changed
        {
            tilemapHighlight.SetTile(currentCellPos, highlightTile); //set the new tile
            tilemapHighlight.SetTile(previousCellPos, null); //put back the previous tiles

            previousCellPos = currentCellPos; // save the new position for next frame
        }
    }*/
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(futurePos, 0.2f);
    }*/
    #endregion
}