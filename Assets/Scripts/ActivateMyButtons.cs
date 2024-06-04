using UnityEngine;
using System.Collections;

public class ActivateMyButtons : MonoBehaviour
{
    public Material yellowButtonMaterial;
    public Material activateMat;
    private Material myMaterial;
    private Material matCopy;
    private bool canUse = true;
    private float cooldownTime = 6f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            myMaterial = renderer.material;
        }
        else
        {
            Debug.LogError("This GameObject does not have a Renderer component.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canUse)
        {
            ActivateButtons();
            StartCoroutine(Cooldown());
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
            StartCoroutine(ActivateButton(button));
        }
    }

    IEnumerator ActivateButton(GameObject button){
        Renderer renderer = button.GetComponent<Renderer>();
        ButtonManager buttonManager = button.GetComponent<ButtonManager>();
        if (renderer != null)
        {   
            matCopy = renderer.material;
            if (renderer.material == myMaterial || renderer.material == yellowButtonMaterial)
            {
                Debug.Log("SDASDASD");
            }
                renderer.material = activateMat;
                Debug.Log($"Button {button.name} activated.");
                
                buttonManager.isActivated = true;
                yield return new WaitForSeconds(2);
                renderer.material = matCopy;
                buttonManager.isActivated = false;
        }
        else
        {
            Debug.LogWarning($"GameObject {button.name} with tag 'Button' does not have a Renderer component.");
        }
    }

    // void DestroyEnemiesInTrigger(GameObject button)
    // {
    //     Transform childTransform = button.transform.GetChild(0); // Replace with the actual name of the child object
    //     if (childTransform != null)
    //     {
    //         Collider collider = Physics.OverlapBox(childTransform.position, childTransform.localScale / 2, childTransform.rotation);
    //         foreach (Collider collider in colliders)
    //         {
    //             if (collider.CompareTag("Enemy"))
    //             {
    //                 Destroy(collider.gameObject);
    //                 score++;
    //                 Debug.Log($"Enemy destroyed. Score: {score}");
    //             }
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Button {button.name} does not have a child object with the expected name.");
    //     }
    // }

    IEnumerator Cooldown()
    {
        canUse = false;
        yield return new WaitForSeconds(cooldownTime);
        canUse = true;
    }
}
