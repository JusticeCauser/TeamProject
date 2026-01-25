using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GadgetDisplayUI : MonoBehaviour
{
    public static GadgetDisplayUI instance;

    [SerializeField] TMP_Text gadgetListText;

    private int currentFlashbangs = 2;
    private int currentSmokes = 2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGadgetDisplay();
    }

    public void UpdateGadgetDisplay()
    {
        if (gadgetInventory.instance == null)
            return;

        // show equipped gadgets
        List<string> gadgets = gadgetInventory.instance.GetEquippedGadgets();

        if(gadgetListText != null)
        {
            string displayText = "EQUIPPED:\n";

            foreach (string gadget in gadgets)
            {
                if (gadget == "Flashbangs")
                    displayText += "Flashbangs (" + currentFlashbangs + ")\n";
                else if (gadget == "Smoke Grenades")
                    displayText += "Smoke Grenades (" + currentSmokes + ")\n";
                else
                    displayText += "" + gadget + "\n";
            }
            gadgetListText.text = displayText;
        }
    }

    public void UpdateThrowableCount(int flashbangs, int smokes)
    {
        currentFlashbangs = flashbangs;
        currentSmokes = smokes;
        UpdateGadgetDisplay();
    }
}
