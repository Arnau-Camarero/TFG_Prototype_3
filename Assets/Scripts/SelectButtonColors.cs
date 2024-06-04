using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SelectButtonColors : NetworkBehaviour
{
    public Material Player1Button;
    public Material Player2Button;

    private GameObject[] buttons;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(!IsServer){
            enabled = false;
            return;
        }

        buttons = GameObject.FindGameObjectsWithTag("Button");
        SetButtonColors();
    }

    void SetButtonColors(){
        int idx = 0;
        foreach (GameObject button in buttons)
        {
            if(idx % 2 == 0){
                button.GetComponent<Renderer>().material = Player1Button;
            }else{
                button.GetComponent<Renderer>().material = Player2Button;
            }
            idx++;
        }
    }
}
