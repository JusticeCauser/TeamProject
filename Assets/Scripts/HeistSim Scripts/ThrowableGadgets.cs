using UnityEngine;

public class ThrowableGadgets : MonoBehaviour
{
    [Header("Settings")]
    public GameObject flashbangPrefab;
    public GameObject smokePrefab;
    public float throwForce = 10f;
    public float effectDuration = 5f;
    public float effectRadius = 5f;

    private int smokesRemaining = 2;
    private int flashbangsRemaining = 2;
    private bool throwingFlash = true;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        bool hasFlash = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget("Flashbangs");
        bool hasSmoke = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget("Smoke Grenades");

        // press 1 to select flashbang
        if(hasFlash && Input.GetKeyDown(KeyCode.Alpha1))
        {
            throwingFlash = true;
        }

        // press 2 for smoke
        if(hasSmoke && Input.GetKeyDown(KeyCode.Alpha2))
        {
            throwingFlash = false;
        }
        // g to throw
        if(Input.GetKeyDown(KeyCode.G))
        {
            if(throwingFlash && hasFlash && flashbangsRemaining > 0)
            {
                flashbangsRemaining--;
                ThrowGadget(flashbangPrefab, "Flashbang");
                UpdateUI();
            }
            else if(!throwingFlash && hasSmoke && smokesRemaining > 0)
            {
                smokesRemaining--;
                ThrowGadget(smokePrefab, "Smoke Grenade");
                UpdateUI();
            }
        }
    }

    void ThrowGadget(GameObject prefab, string gadgetName)
    {
        if(prefab == null)
        {
            return;
        }

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        GameObject effect = Instantiate(prefab, spawnPos, Quaternion.identity);

        Rigidbody rb = effect.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }
        Destroy(effect, effectDuration);
    }

    void UpdateUI()
    {
        if (GadgetDisplayUI.instance != null)
            GadgetDisplayUI.instance.UpdateThrowableCount(flashbangsRemaining, smokesRemaining);
    }
}
