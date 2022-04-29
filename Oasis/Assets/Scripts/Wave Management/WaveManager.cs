using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveManager : MonoBehaviour
{
    public GameObject spawner;
    public GameObject enemy;

    #region Wave Manager Variables
    [SerializeField] private int playerAmount;
    [SerializeField] private int waveNumber;
    [SerializeField] private int initialWaitTimer;
    [SerializeField] private float nextWaveTimer;
    private float e = 2.71828f;
    private int maxWaitTime = 60;
    private int minWaitTime = 20;

    [SerializeField] private float challengeRatingPool;
    [SerializeField] private float enemyChallengeCap;
    private bool isSpawningComplete = true;
    private bool canSpawn = true;
    #endregion

    #region Map Management Variables
    [SerializeField] private MapManager mapManager;
    [SerializeField] private List<Vector3Int> availableTiles;
    [SerializeField] private Vector2 spawnTileCenter;
    public Transform[] spawnPoints;
    #endregion

    #region Enemy Management Variables
    [SerializeField] private GameObject[] enemyPrefabs;
    private int lowestChallengeVal = 1;

    //private int sLength; Spawn points list
    private int eLength;
    #endregion

    #region Adding a New Enemy
    /*
     * Make sure to have its prefab CR set to its desired CR or else this will think it is 0
    */
    #endregion

    private void Awake()
    {
        waveNumber = 0;
        initialWaitTimer = 10; //if this is 0 is messes with waves, make at least 1
        mapManager = GetComponent<MapManager>();
        playerAmount = GameObject.FindGameObjectsWithTag("Player").Length;
        nextWaveTimer = initialWaitTimer;
        spawner = GameObject.Find("Spawner");

        StartCoroutine(WaveTimer(nextWaveTimer));
    }

    private void Update()
    {

        if (nextWaveTimer == 0 && isSpawningComplete) //IDEA: maybe don't force isSpawningComlpelte and only force wait timer
        {
            //if this timer hits 0 and enemies from the previous wave are done spawning reset timer (curerntly min value of 5 seconds and max of 60)
            isSpawningComplete = false;
            nextWaveTimer = Mathf.Min
                (
                Random.Range(minWaitTime, Mathf.Pow(waveNumber, e) + minWaitTime),
                Random.Range(minWaitTime, challengeRatingPool/1000 + minWaitTime)
                );
            GenerateWaveData();
            StartCoroutine(WaveTimer(nextWaveTimer));
        }

        //loop spawning till finished
        if (!isSpawningComplete)
        {
            if (canSpawn)
            {
                NextWave();
            }
        }
    }

    public void GenerateWaveData()
    {
        challengeRatingPool = (Mathf.Pow(playerAmount, (3f / 2f))) * (Mathf.Pow(e, (waveNumber / 2f))) + waveNumber; //equation for getting the appropriate challenge rating pool number
        mapManager.UpdateTileData(); //recheck all tiles (this will need to be change for a range around the player later on)
        availableTiles = mapManager.FindAvailableTiles();
        spawnTileCenter = mapManager.chooseTile(availableTiles);

        spawner.transform.position = spawnTileCenter;
        Debug.Log("Spawner position is: " + spawner.transform.position);

        //TO DO Generate spawnpoints in the healthy tile
        Debug.Log("Challenge rating pool: " + challengeRatingPool);
    }

    public void NextWave()
    {
        enemyChallengeCap = challengeRatingPool / 2; //Limit the Chllenge rating of enemies
        float currentEnemyCR; //This will serve as time as well as the current enemies CR

        if (enemyChallengeCap < 1)
        {
            enemyChallengeCap = 1;
        }

        if (challengeRatingPool >= lowestChallengeVal && canSpawn) //use up challenge rating pool
        {
            //once the pool gets small enough lower the challenge cap with it
            if (enemyChallengeCap > challengeRatingPool)
            {
                enemyChallengeCap = challengeRatingPool;
            }

            currentEnemyCR = SpawnEnemy(enemyChallengeCap); //returns the chosen enemies CR and spawns th enemy
            Debug.Log("Summoned CR: " + currentEnemyCR);
            challengeRatingPool -= currentEnemyCR;
            Debug.Log("Updated Challenge Rating Pool: " + challengeRatingPool);

            if (currentEnemyCR != 0)
            {
                StartCoroutine(SpawnCooldown(currentEnemyCR));
            }//if enemy is chosen
        }//if CR not gone
        else
        {
            isSpawningComplete = true;
            Debug.Log("Wave: " + waveNumber + " spawning is complete...");
        }
    }
    private IEnumerator WaveTimer(float waitTime)
    {
        Debug.Log("###STARTED WAVE TIMER###");
        Debug.Log("Wave " + waveNumber + " is beggining... Wave length is " + waitTime + " seconds...");
        yield return new WaitForSeconds(waitTime);
        nextWaveTimer = 0;
        Debug.Log("Wave " + waveNumber + " has finished...");
        Debug.Log("###FINISHED WAVE TIMER###");
        waveNumber++;
    }

    private IEnumerator SpawnCooldown(float waitTime)
    {
        float preventedTime = waitTime/2;
        canSpawn = false;
        Debug.Log("Preventing spawning for " + preventedTime + " seconds...");
        yield return new WaitForSeconds(preventedTime);
        Debug.Log("Allowing spawning...");
        canSpawn = true;
    }

    public int SpawnEnemy(float challengeCap)
    {
        int randomIndex;
        int challengeRating;

        do
        {
            randomIndex = Random.Range(0, enemyPrefabs.Length);
            challengeRating = (int)enemyPrefabs[randomIndex].GetComponent<EnemyStats>().challengeRating.GetValue;
            //Debug.Log("Challenge Rating " + challengeRating + ", Index: " + randomIndex);
        } while (challengeRating > challengeCap); //reroll until an acceptable challenge rating is selected

        spawner.transform.position = mapManager.GeneratePointInTile(spawnTileCenter);
        Instantiate(enemyPrefabs[randomIndex], spawner.transform.position, Quaternion.identity); //spawn chosen enemy

        return challengeRating;
    }
}
