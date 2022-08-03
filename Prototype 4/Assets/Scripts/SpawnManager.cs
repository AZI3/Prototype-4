using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;

    public GameObject[] powerupPrefab;

    private float SpawnRange = 9.0f;

    public int enemyCount;

    public int waveNumber = 1;

    public GameObject player;

    public GameObject bossPrefab;

    public GameObject[] miniEnemyPrefabs;

    public int bossRound;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        int powerupIndex = Random.Range(0, powerupPrefab.Length);
        Instantiate(powerupPrefab[powerupIndex], GenerateSpawnPosition(), powerupPrefab[powerupIndex].transform.rotation);

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0)
        {
            waveNumber++;
            if (waveNumber % bossRound == 0)
            {
                spawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }

            int powerupIndex = Random.Range(0, powerupPrefab.Length);
            Instantiate(powerupPrefab[powerupIndex], GenerateSpawnPosition(), powerupPrefab[powerupIndex].transform.rotation);
            ResetPlayerPosition();
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-SpawnRange, SpawnRange);
        float spawnPosZ = Random.Range(-SpawnRange, SpawnRange);

        return new Vector3(spawnPosX, 0, spawnPosZ);
    }

    void spawnBossWave(int currentRound)
    {
        int miniEnemiesToSpawn;

        if (bossRound != 0)
        {
            miniEnemiesToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemiesToSpawn = 1;
        }

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemiesToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            int index = Random.Range(0, amount);
            Instantiate(miniEnemyPrefabs[index], GenerateSpawnPosition(), miniEnemyPrefabs[index].transform.rotation);
        }
    }

    private void SpawnEnemyWave(int enemyToSpawn)
    {
        for (int i = 0; i < enemyToSpawn; i++)
        {
            int enemyIndex = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[enemyIndex], GenerateSpawnPosition(), enemyPrefab[enemyIndex].transform.rotation);
        }
    }

    void ResetPlayerPosition()
    {
        player.transform.position = new Vector3(0, 0, 0);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }
}
