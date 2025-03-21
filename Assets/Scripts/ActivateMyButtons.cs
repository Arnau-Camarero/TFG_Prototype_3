using UnityEngine;
using System.Collections;
using Unity.Netcode;
using TMPro;

public class ActivateMyButtons : NetworkBehaviour
{
    public Material yellowButtonMaterial;
    private Material myMaterial;
    private bool canUse = true;
    private float cooldownTime = 6f;

    public TextMeshProUGUI cooldownTextObject;

    private NetworkVariable<float> networkedCountdownTime = new NetworkVariable<float>();

    private NetworkVariable<bool> isActivated = new NetworkVariable<bool>();

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            myMaterial = renderer.material;
        }
    }

    void Update()
    {
        // Solo el Host puede usar ActivateMyButtons
        if (!IsServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && canUse)
        {
            if (IsOwner)
            {
                ActivateButtons();
                StartCoroutine(Cooldown());
                SetIsActivatedServerRpc(true);
            }
            else
            {
                SetIsActivatedServerRpc(true);
            }
        }

        if (isActivated.Value)
        {
            ActivateButtons();
            StartCoroutine(Cooldown());
            isActivated.Value = false;
        }
    }

    void ActivateButtons()
    {
        if (yellowButtonMaterial == null)
        {
            Debug.LogError("Yellow Button Material not assigned.");
            return;
        }

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject button in buttons)
        {
            Renderer renderer = button.GetComponent<Renderer>();
            ButtonManager buttonManager = button.GetComponent<ButtonManager>();

            // Activar todos los botones incondicionalmente si isColorChangeActive es true
            if (ChangeColorButtons.IsActive || renderer.material.name == myMaterial.name || renderer.material.name == yellowButtonMaterial.name)
            {
                buttonManager.ActivateButtonClientRpc();
            }
        }
    }

    IEnumerator Cooldown()
    {
        canUse = false;
        StartCoroutine(StartCooldownCounter(cooldownTime));
        yield return new WaitForSeconds(cooldownTime);
        canUse = true;
    }

    IEnumerator StartCooldownCounter(float cdTime)
    {
        networkedCountdownTime.Value = cdTime;
        while (networkedCountdownTime.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            networkedCountdownTime.Value -= 1f;
            cooldownTextObject.text = networkedCountdownTime.Value.ToString();
        }
    }

    [ServerRpc]
    void SetIsActivatedServerRpc(bool activated)
    {
        isActivated.Value = activated;
        SetIsActivatedClientRpc(activated);
    }

    [ClientRpc]
    void SetIsActivatedClientRpc(bool activated)
    {
        isActivated.Value = activated;
    }
}