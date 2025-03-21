using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    public GameObject objectToSpawn;
    public float minSpawnInterval = 15f;
    public float maxSpawnInterval = 20f;
    public int maxEnemies = 1;
    public List<GameObject> enemyCounter;
    public float minZOffset = -5.0f;
    public float maxZOffset = 5.0f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);

            if (NetworkManager.Singleton.ConnectedClients.Count >= 2 && enemyCounter.Count < maxEnemies)
            {
                SpawnEnemy();
            }           
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position;
        GameObject enemy = Instantiate(objectToSpawn, spawnPosition, transform.rotation);

        NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(true);
        }

        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        if (enemyBehaviour != null)
        {
            enemyBehaviour.spawner = this;
        }

        enemyCounter.Add(enemy);
    }
}
