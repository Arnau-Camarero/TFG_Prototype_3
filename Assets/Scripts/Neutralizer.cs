using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Neutralizer : MonoBehaviour
{
    public bool executing;
    public GameObject score = GameObject.Find("Score");

    void Start()
    {
        if (score == null)
        {
            score = GameObject.Find("Score");
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (executing && col.transform.CompareTag("Enemy"))
        {
            // Call method from score EnemyDestroyedByButton()
            score.GetComponent<Score>().EnemyDestroyedByButton();
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
