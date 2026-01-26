using UnityEngine;
using System.Collections.Generic;

public class gadgetInventory : MonoBehaviour
{
    public static gadgetInventory instance;
    private List<string> equippedGadgets = new List<string>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGadgets();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //editing to push
    void LoadGadgets()
    {
        // first check scene, if gadgetryScene always give all gadgets
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if(currentScene == "gadgetryScene")
        {
            equippedGadgets = new List<string>
            {
                "Grapple",
                "Lockpick",
                "Glass Cutter",
                "Fingerprint Lifter",
                "Code Hacker",
                "Camera Jammer",
                "Pocket Mirrors",
                "Flashbangs",
                "Smoke Grenades",
                "Scent Masker"
            };
        }
        else
        {
            // in missions, load from hub
            string gadgetData = PlayerPrefs.GetString("SelectedGadgets", "");
            if(!string.IsNullOrEmpty(gadgetData))
            {
                equippedGadgets = new List<string>(gadgetData.Split(','));
            }
        }
        //string gadgetData = PlayerPrefs.GetString("SelectedGadgets", "");

        //if (!string.IsNullOrEmpty(gadgetData))
        //{
        //    equippedGadgets = new List<string>(gadgetData.Split(','));
        //}
        //else
        //{
        //    // give all gadgets for gadgetryScene testing
        //    equippedGadgets = new List<string> { "Grappler", "Lockpick", "Glass Cutter", "Fingerprint Lifter", "Code Hacker", "Camera Jammer", "Pocket Mirrors" };
        //}
    }

    public bool HasGadget(string gadgetName)
    {
        return equippedGadgets.Contains(gadgetName);
    }

    public List<string> GetEquippedGadgets()
    {
        return new List<string>(equippedGadgets);
    }
}