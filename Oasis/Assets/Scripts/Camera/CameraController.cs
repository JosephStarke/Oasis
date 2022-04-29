using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Panning Variables
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float borderThickness = 10f; //variable for mouse to get close to edge of screen
    [SerializeField] private Vector2 panLimit; //Map limits/edge
    #endregion

    #region Center player Variables
    [SerializeField] GameObject player;
    [SerializeField] private bool centered = true;
    #endregion

    #region Camera Zoom Variables
    [SerializeField] private new Camera camera;
    [SerializeField] private float minFov = 2;
    [SerializeField] private float maxFov = 50;
    [SerializeField] private float zoomSpeed = 3;
    #endregion

    private void Awake()
    {
        //grab camera and PlayerStats objects
        camera = Camera.main;
        player = GameObject.Find("Player");
        //make sure camera is set properly
        UpdateFov(camera.orthographicSize);
        CenterToPlayer();
    }
    private void Update()
    {
        //if space is pressed or being held panning is impossible and it will center on the PlayerStats
        if (Input.GetKeyDown("space") && centered == true)
        {
            centered = false;
        }
        else if (Input.GetKeyDown("space") && centered == false)
        {
            centered = true;
        }

        if (centered) //for hold -> !Input.GetKey("space") 
        {
            CenterToPlayer();
        }
        else  //for hold Input.GetKey("space")
        {
            Pan();
        }

        Zoom();
    }

    private void Pan()
    {
        //get current camera pos
        Vector3 pos = transform.position;
        //Mouse to move positions
        if (Input.mousePosition.y >= Screen.height - borderThickness) //UP
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= borderThickness) //DOWN
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - borderThickness) //RIGHT
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= borderThickness) //LEFT
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        //set border limits
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);
        //move the camera
        transform.position = pos;
    }

    private void CenterToPlayer()
    {
        Vector3 pos = player.transform.position;
        pos.z -= 10;
        transform.position = pos;
    }

    #region Zoom Functions
    private void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0)
        {
            UpdateFov(camera.orthographicSize - zoomSpeed);
        }
        else if (scrollInput < 0)
        {
            UpdateFov(camera.orthographicSize + zoomSpeed);
        }
    }

    private void UpdateFov(float newFov)
    {
        camera.orthographicSize = Mathf.Clamp(newFov, minFov, maxFov);
    }
    #endregion 

}