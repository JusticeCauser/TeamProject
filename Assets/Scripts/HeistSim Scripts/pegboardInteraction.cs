using UnityEngine;
using TMPro;

public class pegboardInteraction : MonoBehaviour
{
    private hubManager hubManager;
    private bool playerInRange = false;

    [SerializeField] TMP_Text promptText;
    [SerializeField] GameObject backButton;

    // pegboard UI view
    [SerializeField] GameObject loadoutPanel;
    // later, grayed out gear, purchase options (maybe from PC, need to discuss)

    private void Start()
    {
        hubManager = FindFirstObjectByType<hubManager>();

        if (promptText != null)
            promptText.gameObject.SetActive(false);
        if (loadoutPanel != null)
            loadoutPanel.SetActive(false);
    }

    private void Update()
    {
        
    }

}
