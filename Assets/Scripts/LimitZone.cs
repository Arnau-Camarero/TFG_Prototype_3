using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LimitZone : MonoBehaviour
{
    public GameObject score;

    void Start()
    {
        if (score == null)
        {
            score = GameObject.Find("Score");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Button")
        {
            return;
        }
        if (col.gameObject.tag == "Enemy")
        {
            score.GetComponent<Score>().EnemyDestroyedByLimitZone();
            DestroyEnemyServerRpc(col.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyEnemyServerRpc(ulong enemyNetworkObjectId)
    {
        NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkObjectId];
        if (enemyNetworkObject != null)
        {
            Destroy(enemyNetworkObject.gameObject);
        }
    }
}
