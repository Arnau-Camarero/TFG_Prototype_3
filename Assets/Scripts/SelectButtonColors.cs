using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class SelectButtonColors : NetworkBehaviour
{
    public Material Player1Button;
    public Material Player2Button;

    private GameObject[] buttons;
    public NetworkVariable<FixedString128Bytes> buttonAssignments = new NetworkVariable<FixedString128Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            buttons = GameObject.FindGameObjectsWithTag("Button");
            SetButtonColors();
            
            UpdateButtonColorsClientRpc(buttonAssignments.Value);
        }
        else
        {
            buttons = GameObject.FindGameObjectsWithTag("Button");
            ApplyButtonColors(buttonAssignments.Value);
        }
    }

    void SetButtonColors()
    {
        int idx = 0;
        string assignments = "";
        foreach (GameObject button in buttons)
        {
            int buttonNumber = int.Parse(button.name);
            Material assignedMaterial = (buttonNumber % 2 == 0) ? Player1Button : Player2Button;
            button.GetComponent<ButtonManager>().matCopy = assignedMaterial;
            button.GetComponent<Renderer>().material = assignedMaterial;
            assignments += (idx % 2 == 0) ? "1" : "2";
            idx++;
        }
        buttonAssignments.Value = assignments;
    }

    [ClientRpc]
    void UpdateButtonColorsClientRpc(FixedString128Bytes assignments)
    {
        ApplyButtonColors(assignments);
    }

    public void ApplyButtonColors(FixedString128Bytes assignments)
    {
        for (int idx = 0; idx < buttons.Length; idx++)
        {
            int buttonNumber = int.Parse(buttons[idx].name);
            Material assignedMaterial = (buttonNumber % 2 == 0) ? Player1Button : Player2Button;
            buttons[idx].GetComponent<ButtonManager>().matCopy = assignedMaterial;
            buttons[idx].GetComponent<Renderer>().material = assignedMaterial;
        }
    }
}
