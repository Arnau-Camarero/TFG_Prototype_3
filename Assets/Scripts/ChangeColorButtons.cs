using UnityEngine;
using System.Collections;

public class ChangeColorButtons : MonoBehaviour
{
    public Material yellowButtonMaterial;
    private bool canUse = true;
    private float cooldownTime = 10f;
    public static bool IsActive { get; private set; } = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canUse)
        {
            StartCoroutine(ChangeButtonColorsTemporarily());
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator ChangeButtonColorsTemporarily()
    {
        IsActive = true;

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        Material[] originalMaterials = new Material[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            Renderer renderer = buttons[i].GetComponent<Renderer>();
            ButtonManager buttonManager = buttons[i].GetComponent<ButtonManager>();
            if (renderer != null)
            {
                if(buttonManager.isActivated){
                    originalMaterials[i] = buttonManager.matCopy;
                }else{
                    originalMaterials[i] = renderer.material;
                }
                renderer.material = yellowButtonMaterial;
            }
        }
        
        yield return new WaitForSeconds(5);

        for (int i = 0; i < buttons.Length; i++)
        {
            Renderer renderer = buttons[i].GetComponent<Renderer>();
            if (renderer != null && originalMaterials[i] != null)
            {
                renderer.material = originalMaterials[i];
            }
        }

        IsActive = false;
    }

    IEnumerator Cooldown(){

        canUse = false;
        yield return new WaitForSeconds(cooldownTime);
        canUse = true;
        
    }
}
