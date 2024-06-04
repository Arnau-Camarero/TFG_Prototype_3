using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutralizer : MonoBehaviour
{
    public bool executing;
    
    void OnTriggerStay(Collider col){
        if(executing && col.transform.CompareTag("Enemy")){
            Destroy(col.gameObject);
        }
    }
}
