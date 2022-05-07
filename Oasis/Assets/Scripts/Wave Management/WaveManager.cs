using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject spawner;
    public GameObject infectedIndicator;
    public GameObject player;

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
    public GameObject currentSpawner;
    public List<GameObject> spawners;
    #endregion

    #region Adding a New Enemy
    /*
     * Make sure to have its prefab CR set to its desired CR or else this will think it is 0
    */
    #endregion

    private void Awake()
    {
        waveNumber = 0;
        initialWaitTimer = 1; //if this is 0 is messes with waves, make at least 1
        mapManager = GetComponent<MapManager>();
        playerAmount = GameObject.FindGameObjectsWithTag("Player").Length;
        nextWaveTimer = initialWaitTimer;
        //spawner = GameObject.Find("Spawner");
        player = GameObject.Find("Player");

        StartCoroutine(WaveTimer(nextWaveTimer));
    }

    private void Update()
    {

        if (nextWaveTimer == 0) //IDEA: maybe don't force isSpawningComlpelte and only force wait timer
        {
            //if this timer hits 0 and enemies from the previous wave are done spawning reset timer
            //nextWaveTimer = 3f;
            nextWaveTimer = Mathf.Min
                (
                Random.Range(minWaitTime, Mathf.Pow(waveNumber, e) + minWaitTime),
                Random.Range(minWaitTime, challengeRatingPool/1000 + minWaitTime)
                );
            GenerateWaveData();
            StartCoroutine(WaveTimer(nextWaveTimer));

            if (waveNumber > 0)
            {
                currentSpawner = Instantiate(spawner, spawnTileCenter, Quaternion.identity);
                currentSpawner.GetComponent<Wave>().wave = ConstructWave();
                currentSpawner.GetComponent<Wave>().tileCenter = spawnTileCenter;

                spawners.Add(currentSpawner);
            }
        }
    }

    public void GenerateWaveData()
    {
        challengeRatingPool = (Mathf.Pow(playerAmount, (3f / 2f))) * (Mathf.Pow(e, (waveNumber / 2f))) + waveNumber; //equation for getting the appropriate challenge rating pool number
        mapManager.UpdateTileData(); //recheck all tiles (this will need to be change for a range around the player later on)
        availableTiles = mapManager.FindAvailableTiles();
        spawnTileCenter = mapManager.ChooseTile(availableTiles);

        Debug.Log("Challenge rating pool: " + challengeRatingPool);
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

    public GameObject[] ConstructWave()
    {
        enemyChallengeCap = challengeRatingPool / 2; //Limit the Chllenge rating of enemies


        List<GameObject> wave = new List<GameObject>();
        GameObject currentEnemy;

        if (enemyChallengeCap < 1)
        {
            enemyChallengeCap = 1;
        }

        while (challengeRatingPool >= lowestChallengeVal) //use up challenge rating pool
        {
            //once the pool gets small enough lower the challenge cap with it
            if (enemyChallengeCap > challengeRatingPool)
            {
                enemyChallengeCap = challengeRatingPool;
            }
            currentEnemy = ChooseEnemy(enemyChallengeCap); //returns the chosen enemies CR and spawns th enemy
            wave.Add(currentEnemy);

            Debug.Log("Chosen CR: " + currentEnemy.GetComponent<EnemyStats>().challengeRating.GetValue);
            challengeRatingPool -= currentEnemy.GetComponent<EnemyStats>().challengeRating.GetValue;
            Debug.Log("Updated Challenge Rating Pool: " + challengeRatingPool);

        }//if CR not gone

        Debug.Log("Wave: " + waveNumber + " selection is complete...");

        return wave.ToArray();
    }

    public GameObject ChooseEnemy(float challengeCap)
    {
        int randomIndex;
        int challengeRating;
        GameObject enemy;

        do
        {
            randomIndex = Random.Range(0, enemyPrefabs.Length);
            challengeRating = (int)enemyPrefabs[randomIndex].GetComponent<EnemyStats>().challengeRating.GetValue;
            //Debug.Log("Challenge Rating " + challengeRating + ", Index: " + randomIndex);
        } while (challengeRating > challengeCap); //reroll until an acceptable challenge rating is selected

        enemy = enemyPrefabs[randomIndex];

        return enemy;
    }
}
