/******************************************************************************
 * Author: Brad Dixon
 * File Name: EnemySpawner.cs
 * Creation Date: 9/3/2026
 * Last Modified: 9/3/2026
 * Brief: Spawns enemies in waves throughout spawn points
 * External Resources: https://discussions.unity.com/t/list-of-lists/865534/4
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;

[System.Serializable]
public struct EnemyWaveCount<T>
{
    public List<T> list;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] List<WaveData> enemySpawnPatterns = new List<WaveData>();
    [SerializeField] int testPoints;
    [SerializeField] int testWaves;
    [Tooltip("This is so enemies don't just all spawn on top of each other.")]
    [SerializeField] float spawnDelay;
    int waveIndex;
    int totalWaves;
    int totalPoints;
    bool allEnemiesSpawned;

    [SerializeField] int enemyCount;

    int pointsLeftForSpawning;

    [Button("Test Wave Index")]
    private void TestWaveIndex()
    {
        StartEncounter(testPoints, testWaves);
    }

    [Button("Test kill enemies.")]
    private void TestKillAllEnemies()
    {
        enemyCount = 0;
        RemoveEnemy();
    }

    public void StartEncounter(int roomPoints, int roomWaves)
    {
        totalPoints = roomPoints;
        totalWaves = roomWaves;
        waveIndex = 0;
        allEnemiesSpawned = false;
        StartCoroutine(SpawnWave(roomPoints));
    }

    IEnumerator SpawnWave(int pointsForSpawning)
    {
        testPoints = pointsForSpawning;
        for(int i = pointsForSpawning; i > 0;)
        {
            bool enemyFound = false;
            int randomIndex = 0;
            int breakoutCount = 0;
            while (!enemyFound)
            {
                randomIndex = Random.Range(0, enemySpawnPatterns.Count);

                if(i - enemySpawnPatterns[randomIndex].pointCostToSpawn >= 0)
                {
                    i -= enemySpawnPatterns[randomIndex].pointCostToSpawn;
                    enemyFound = true;
                }
                else
                {
                    ++breakoutCount;
                }
            }

            WaveData temp = enemySpawnPatterns[randomIndex];
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

            Debug.Log("Using " + temp.pointCostToSpawn + " points!!");
            testPoints -= temp.pointCostToSpawn;
            Debug.Log("I have " + testPoints + "left!!");
        }

        allEnemiesSpawned = true;
        ++waveIndex;
    }

    Vector3 GetSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex].transform.position;
    }

    public void RemoveEnemy()
    {
        --enemyCount;

        if(allEnemiesSpawned && enemyCount <= 0 && waveIndex < totalWaves)
        {
            StartCoroutine(SpawnWave(totalPoints));
        }
    }
}
