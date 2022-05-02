using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{   
    [SerializeField] private MapManager mapManager;
    [SerializeField] private PlayerMovement playerMovement;

    public GameObject[] wave;
    public List<GameObject> enemies;
    public Vector2 tileCenter;
    public GameObject player;
    public GameObject infectedTileIndicator;
    public float trapInitializationTime;
    public float forcedMoveTime;

    private bool canSpawn;
    private bool spawning;
    private bool checking;
    private int index;

    /*#region Constructors
    public Wave(List<GameObject> wave, Vector2 tileCenter)
    {
        this.wave = wave;
        this.tileCenter = tileCenter;
    }

    public Wave(List<GameObject> wave, Vector3 tileCenter)
    {
        this.wave = wave;
        this.tileCenter = tileCenter;
    }
    #endregion*/

    private void Awake()
    {
        player = GameObject.Find("Player");
        mapManager = GameObject.Find("GridManager").GetComponent<MapManager>();

        playerMovement = player.GetComponent<PlayerMovement>();

        infectedTileIndicator = Instantiate(infectedTileIndicator, transform.position, Quaternion.identity);

        trapInitializationTime = 0.05f;
        forcedMoveTime = 0.05f;

        canSpawn = true;
        spawning = false;
        checking = true;
        index = 0;
    }

    private void Update()
    {
        if (checking)
        {
            CheckPlayerEnter();
        }

        if (spawning)
        {
           spawning = SpawnEnemies(); //returns true if still able to spawn and false if not
        }

        if (!checking && !spawning) //wave has already completed spawning
        {
            if (CheckWaveCleared() == true)
            {
                mapManager.DestroyTileWalls();
                Destroy(this.gameObject);
            }
        }
    }
    public void CheckPlayerEnter()
    {
        //Check if player eneters the infected tile
        if (mapManager.CheckIfSameTile(player.transform.position, transform.position))
        {
            checking = false;
            Debug.Log("Player: " + player + "entered infected tile: " + transform.position);
            Debug.Log("Commencing Spawing");
            //Destroy indicator
            Destroy(infectedTileIndicator);

            //Force player into the edges
            playerMovement.ForceMove(forcedMoveTime);

            //Set up walls
            StartCoroutine(TrapPlayer(trapInitializationTime));

            //Initialization time before spawning starts

            //Spawn enemies in random places
            spawning = true;
        }
    }

    public bool CheckWaveCleared()
    {
        int deathCount = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                deathCount++; //increment death count if all enemies are dead
            }
        }

        if (deathCount == wave.Length)
        {
            return true;
        }

        return false;
    }

    private bool SpawnEnemies()
    {
        if (index < wave.Length)
        {
            if (canSpawn)
            {
                transform.position = mapManager.GeneratePointInTile(tileCenter);
                //Debug.LogError("Generated Point: " + transform.position);

                enemies.Add(Instantiate(wave[index], transform.position, Quaternion.identity)); //spawn enemy at a random point and ad it to enemies list (to check if they are alive or not)
                int challengeRating = (int)wave[index].transform.root.GetComponent<EnemyStats>().challengeRating.GetValue;

                //Debug.LogError("Challenge Rating: " + challengeRating);
                StartCoroutine(SpawnCooldown(challengeRating));
                index++;
            }

            return true;
        }
        else
        {
            //Destroy(this.gameObject);
        }
        return false;
    }

    
    private IEnumerator SpawnCooldown(float waitTime)
    {
        float preventedTime = waitTime / 2;
        canSpawn = false;
        Debug.Log("Preventing spawning for " + preventedTime + " seconds...");

        yield return new WaitForSeconds(preventedTime);

        Debug.Log("Allowing spawning...");
        canSpawn = true;
    }

    private IEnumerator TrapPlayer(float waitTime)
    {
        Debug.Log("Will trap player in " + waitTime + " seconds...");

        yield return new WaitForSeconds(waitTime);
        mapManager.GenerateTileWalls(transform.position);

        Debug.Log("Trap complete.");
    }
}
