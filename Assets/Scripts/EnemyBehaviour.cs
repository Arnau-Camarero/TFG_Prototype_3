using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBehaviour : NetworkBehaviour
{
    private Rigidbody rb;
    
    public Spawner spawner;
    
    public float speed = 8.0f;
    private float initialSpeed;

    public float timeAcumulated = 0;

    public override void OnNetworkSpawn(){
        if(!IsServer){
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        initialSpeed = speed;
    }

    void Update()
    {
        timeAcumulated += (Time.time/100000000);
        speed = initialSpeed + (timeAcumulated / 100000000);
        Vector3 direction = transform.forward.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + direction);
    }

    public override void OnDestroy()
    {
        if (spawner != null && spawner.enemyCounter != null)
        {
            spawner.enemyCounter.Remove(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Spawner or enemyCounter is null in EnemyBehaviour.OnDestroy()");
        }
    }

}
