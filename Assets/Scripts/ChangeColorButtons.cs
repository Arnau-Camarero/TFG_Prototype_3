using UnityEngine;
using System.Collections;
using Unity.Netcode;
using TMPro;

public class ChangeColorButtons : NetworkBehaviour
{
    public Material yellowButtonMaterial;
    private bool canUse = true;
    private float cooldownTime = 20f;
    public static bool IsActive { get; private set; } = false;

    private NetworkVariable<bool> isColorChangeActive = new NetworkVariable<bool>();

    public TextMeshProUGUI cooldownTextObject;

    public override void OnNetworkSpawn()
    {
        
        if (!IsServer)
        {
            cooldownTextObject = GameObject.FindGameObjectWithTag("CountDown").GetComponent<TextMeshProUGUI>();
            cooldownTextObject.text = "E: Skill Ready!";
            cooldownTextObject.color = Color.green;

        }
    }

    void Update()
    {
        // Solo el cliente puede usar ChangeColorButtons
        if (IsServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && canUse)
        {
            if (IsOwner)
            {
                StartCoroutine(ChangeButtonColorsTemporarily());
                StartCoroutine(Cooldown());
                RequestColorChangeServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestColorChangeServerRpc()
    {
        isColorChangeActive.Value = true;
        TriggerColorChangeClientRpc();
    }

    [ClientRpc]
    void TriggerColorChangeClientRpc()
    {
        StartCoroutine(ChangeButtonColorsTemporarily());
    }

    IEnumerator ChangeButtonColorsTemporarily()
    {
        IsActive = true;

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        Material[] originalMaterials = new Material[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            Renderer renderer = buttons[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[i] = renderer.material;
                renderer.material = yellowButtonMaterial;
            }
        }

        yield return new WaitForSeconds(6);

        for (int i = 0; i < buttons.Length; i++)
        {
            Renderer renderer = buttons[i].GetComponent<Renderer>();
            if (renderer != null && originalMaterials[i] != null)
            {
                renderer.material = originalMaterials[i];
            }
        }

        SelectButtonColors selectButtonColors = FindObjectOfType<SelectButtonColors>();
        if (selectButtonColors != null)
        {
            selectButtonColors.ApplyButtonColors(selectButtonColors.buttonAssignments.Value);
        }

        IsActive = false;
    }

    IEnumerator Cooldown()
    {
        canUse = false;
        float remainingTime = cooldownTime;
        
        cooldownTextObject.color = Color.red;
        while (remainingTime > 0)
        {
            cooldownTextObject.text = $"E: Skill in {remainingTime}s";
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        cooldownTextObject.text = "E: Skill Ready!";
        cooldownTextObject.color = Color.green;
        canUse = true;
    }
}