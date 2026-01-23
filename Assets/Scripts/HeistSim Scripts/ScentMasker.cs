using TMPro;
using UnityEngine;

public class ScentMasker : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Scent Masker";
    public float effectDuration = 10f;
    //public float effectRadius = 5f;

    [Header("UI")]
    public TMP_Text promptText;

    private bool hasGadget = false;
    private bool effectActive = false;
    private float effectTimer = 0f;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        if (!hasGadget)
            return;

        if(effectActive)
        {
            effectTimer -= Time.deltaTime;

            if (promptText != null)
                promptText.text = "Scent Masker Active for " + Mathf.CeilToInt(effectTimer) + "s";

            if(effectTimer <= 0)
            {
                effectActive = false;
                if (promptText != null)
                    promptText.text = "";
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

        if (promptText != null)
            promptText.gameObject.SetActive(true);
        // prevent dog detection when active
    }

    public bool IsMaskerActive()
    {
        return effectActive;
    }
}
