/******************************************************************************
 * Author: Brad Dixon
 * File Name: EnemySpawner.cs
 * Creation Date: 9/3/2026
 * Last Modified: 9/4/2026
 * Brief: Spawns enemies in waves throughout spawn points
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Where the enemies can spawn.")]
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [Tooltip("The list of patterns the enemies can spawn in. Each pattern has a cost.")]
    [SerializeField] List<WaveData> enemySpawnPatterns = new List<WaveData>();
    [Tooltip("All the doors in an encounter room.")]
    [SerializeField] List<GameObject> roomDoors = new List<GameObject>();

    //These two variables are testing variables in the inspector
    [SerializeField] int testPoints;
    [SerializeField] int testWaves;


    [Tooltip("This is so enemies don't just all spawn on top of each other.")]
    [SerializeField] float spawnDelay;
    int waveIndex;
    int totalWaves;
    int totalPoints;
    bool allEnemiesSpawned;

    bool cleared;

    //Doesn't need to be serialized, just is so we can keep track of enemy count during run time
    [SerializeField] int enemyCount;

    [Button("Test Wave Index")]
    /// <summary>
    /// Testing button for starting the encounter
    /// </summary>
    private void TestWaveIndex()
    {
        StartEncounter(testPoints, testWaves);
    }

    /// <summary>
    /// Testing button that simulates killing all the enemies. Doesn't actually kill the enemy game objects
    /// </summary>
    [Button("Test kill enemies.")]
    private void TestKillAllEnemies()
    {
        enemyCount = 0;
        RemoveEnemy();
    }

    /// <summary>
    /// Makes it so encounter rooms spawn not cleared
    /// </summary>
    private void Start()
    {
        cleared = false;
        foreach(GameObject g in roomDoors)
        {
            g.SetActive(false);
        }
    }

    /// <summary>
    /// This will be the public call a room should use to start an encounter
    /// </summary>
    /// <param name="roomPoints"></param> How many points the encounter has to spawn enemies
    /// <param name="roomWaves"></param> How many waves the encounter has
    public void StartEncounter(int roomPoints, int roomWaves)
    {
        //Only start the encounter if it already hasn't been cleared
        if (!cleared)
        {
            totalPoints = roomPoints;
            totalWaves = roomWaves;
            waveIndex = 0;
            allEnemiesSpawned = false;
            StartCoroutine(SpawnWave(roomPoints));
        }
    }

    /// <summary>
    /// Spawns all the enemies for one wave of the encounter.
    /// </summary>
    /// <param name="pointsForSpawning"></param> How many points the wave has for spawning
    /// <returns></returns>
    IEnumerator SpawnWave(int pointsForSpawning)
    {
        testPoints = pointsForSpawning;

        //Loop until we run out of points for spawning
        for(int i = pointsForSpawning; i > 0;)
        {
            bool enemyFound = false;
            int randomIndex = 0;

            //Loop until we find an enemy pattern we can spawn.
            //This makes sure we don't spawn patterns that cost too much.
            while (!enemyFound)
            {
                randomIndex = Random.Range(0, enemySpawnPatterns.Count);

                if(i - enemySpawnPatterns[randomIndex].pointCostToSpawn >= 0)
                {
                    i -= enemySpawnPatterns[randomIndex].pointCostToSpawn;
                    enemyFound = true;
                }
            }

            //Reference so we don't have to keep writing it out
            WaveData temp = enemySpawnPatterns[randomIndex];

            //Loops until all enemies in the chosen pattern have been spawned
            for(int j = temp.enemyCount; j > 0; --j)
            {
                int randomEnemyIndex = Random.Range(0, temp.enemyTypes.Count);

                GameObject enemy = Instantiate(temp.enemyTypes[randomEnemyIndex], GetSpawnPoint(), Quaternion.identity);

                if(enemy.GetComponent<EnemyBehaviour>())
                {
                    enemy.GetComponent<EnemyBehaviour>().SetSpawnerReference(this);
                }
                else
                {
                    enemy.GetComponent<DummyBehaviour>().SetSpawnerReference(this);
                }

                ++enemyCount;

                yield return new WaitForSeconds(spawnDelay);
            }

            //Debugging to make sure everything works as intended. Not necessary for code.
            Debug.Log("Using " + temp.pointCostToSpawn + " points!!");
            testPoints -= temp.pointCostToSpawn;
            Debug.Log("I have " + testPoints + "left!!");
        }

        //Updates values for determining if the next wave should be spawned
        allEnemiesSpawned = true;
        ++waveIndex;
    }

    /// <summary>
    /// Randomly selects one of the enemy spawn points in the room
    /// </summary>
    /// <returns></returns> Returns the randomly selected spawn point
    Vector3 GetSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex].transform.position;
    }

    /// <summary>
    /// Enemies call this when they die. Let's us know when to spawn the next wave
    /// </summary>
    public void RemoveEnemy()
    {
        --enemyCount;

        //Makes sure that all enemies in a wave have been killed and that there are still other waves to call
        if (allEnemiesSpawned && enemyCount <= 0)
        {
            if (waveIndex < totalWaves)
            {
                StartCoroutine(SpawnWave(totalPoints));
            }
            else
            {
                //Makes it so the room won't start an encounter if backtracked to
                cleared = true;

                //Enables the room doors when the encounter is cleared
                foreach (GameObject g in roomDoors)
                {
                    g.SetActive(true);
                }
            }
        }
    }
}
