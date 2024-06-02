using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LimitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Button"){
            return;
        }
        if(col.gameObject.tag == "Enemy"){
            Destroy(col.gameObject);
        }
    }
}
