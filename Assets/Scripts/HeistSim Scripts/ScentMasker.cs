using UnityEngine;

public class ScentMasker : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Scent Masker";
    public float effectDuration = 10f;
    public float effectRadius = 5f;

    private bool hasGadget = false;
    private bool effectActive = false;
    private float effectTimer = 0f;

    private void Update()
    {
        hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        if (!hasGadget)
            return;

        if(effectActive)
        {
            effectTimer -= Time.deltaTime;
            if(effectTimer <= 0)
            {
                effectActive = false;
            }
        }

        if(!effectActive && Input.GetKeyDown(KeyCode.H))
        {
            ActivateMasker();
        }
    }

    void ActivateMasker()
    {
        effectActive = true;
        effectTimer = effectDuration;

        // prevent dog detection when active
    }

    public bool IsMaskerActive()
    {
        return effectActive;
    }
}
