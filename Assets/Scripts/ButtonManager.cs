using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ButtonManager : NetworkBehaviour
{
    public Material matCopy;
    public Material originalButtonMaterial;
    public Material yellowButtonMaterial;
    public Material activateMat;
    public bool isActivated = false;
    private Neutralizer trigger;
    private Renderer buttonrenderer;
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        buttonrenderer = GetComponent<Renderer>();
        trigger = transform.GetChild(0).gameObject.GetComponent<Neutralizer>();
        matCopy = buttonrenderer.material;
        originalButtonMaterial = buttonrenderer.material;
    }

    void Update()
    {
        if (isPressed.Value)
        {
            StartCoroutine(ActivateButton());
        }
        if (isActivated)
        {
            trigger.executing = true;
        }
        else
        {
            trigger.executing = false;
        }
    }

    public IEnumerator ActivateButton()
    {
        buttonrenderer.material = activateMat;
        isActivated = true;
        yield return new WaitForSeconds(1);

        // Check if the color change ability is active
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
        if (isColorChangeActive)
        {
            buttonrenderer.material = yellowButtonMaterial;
        }
        else
        {
            if (matCopy.name == yellowButtonMaterial.name)
            {
                // Get the original material
                buttonrenderer.material = originalButtonMaterial;
            }
            else
            {
                buttonrenderer.material = matCopy;
            }
        }

        isActivated = false;
        SetIsPressedServerRpc(false);
    }

    [ClientRpc]
    public void ActivateButtonClientRpc()
    {
        StartCoroutine(ActivateButton());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Player") && !isActivated && IsServer)
        {
            if ((col.gameObject.GetComponent<Renderer>().material.name == buttonrenderer.material.name) || (buttonrenderer.material.name == (yellowButtonMaterial.name + " (Instance)")))
            {
                matCopy = buttonrenderer.material;
                SetIsPressedServerRpc(true);
            }
        }
    }

    [ServerRpc]
    void SetIsPressedServerRpc(bool pressed)
    {
        isPressed.Value = pressed;
        SetIsPressedClientRpc(pressed);
    }

    [ClientRpc]
    void SetIsPressedClientRpc(bool pressed)
    {
        isPressed.Value = pressed;
    }
}