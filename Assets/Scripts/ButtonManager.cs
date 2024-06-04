using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ButtonManager : NetworkBehaviour
{
    public Material matCopy;
    public Material yellowButtonMaterial;
    public Material activateMat;
    public bool isActivated = false;
    private Neutralizer trigger;
    private Renderer buttonrenderer;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(!IsOwner){
            enabled= false;
            return;
        }

        buttonrenderer = GetComponent<Renderer>();
        trigger = transform.GetChild(0).gameObject.GetComponent<Neutralizer>();
        matCopy = buttonrenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActivated){
            trigger.executing = true;
        }else{
            trigger.executing = false;
        }
    }

    IEnumerator ActivateButton(){

        buttonrenderer.material = activateMat;
        isActivated = true;
        yield return new WaitForSeconds(1);
        
        // Ver si la habilidad de cambio de color esta activa
        bool isColorChangeActive = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            ChangeColorButtons colorChanger = player.GetComponent<ChangeColorButtons>();
            if (colorChanger != null && ChangeColorButtons.IsActive)
            {
                isColorChangeActive = true;
                break;
            }
        }
        if (isColorChangeActive){
            buttonrenderer.material = yellowButtonMaterial;
        }
        else{
            buttonrenderer.material = matCopy;
        }
        
        isActivated = false;
    }

    void OnTriggerEnter(Collider col){
        if(col.transform.CompareTag("Player") && !isActivated){
            Debug.Log(buttonrenderer.material.name);
            Debug.Log(yellowButtonMaterial.name);            
            if((col.gameObject.GetComponent<Renderer>().material.name == buttonrenderer.material.name) || (buttonrenderer.material.name == (yellowButtonMaterial.name + " (Instance)"))){
                Debug.Log("IN: ACTIVATED");
                matCopy = buttonrenderer.material;
                StartCoroutine(ActivateButton());
            }
        }
    }
}
