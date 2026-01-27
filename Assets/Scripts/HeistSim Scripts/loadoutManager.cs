using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class loadoutManager : MonoBehaviour
{
    [Header("Tool Buttons")]
    public Button[] toolButtons; //needs placeholders assigned, 10 gadgets so far

    [Header("UI")]
    public GameObject loadoutConfirmPanel;
    public GameObject loadoutPanel;

    [Header("Selection Settings")]
    public int maxTools = 3; // maybe need to be default 4, but for now 3 for easier testing
    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;

    private List<string> selectedToolNames = new List<string>();
    private List<int> selectedToolIndices = new List<int>();

    private void Start()
    {
        // set up button listeners
        for (int i = 0; i < toolButtons.Length; i++)
        {
            int index = i; // capture index
            toolButtons[i].onClick.AddListener(() => ToggleTool(index));
        }
    }

    void ToggleTool(int toolIndex)
    {
        // only allow selection if in mission flow
        hubManager hm = FindFirstObjectByType<hubManager>();
        if (hm != null && !hm.IsInMissionFlow())
            return;

        // get tool name from button text
        string toolName = toolButtons[toolIndex].GetComponentInChildren<TMP_Text>().text;

        if (selectedToolIndices.Contains(toolIndex))
        {
            //deselect
            selectedToolIndices.Remove(toolIndex);
            selectedToolNames.Remove(toolName);
            toolButtons[toolIndex].GetComponent<Image>().color = unselectedColor;
        }
        else
        {
            // select if under max
            if (selectedToolIndices.Count < maxTools)
            {
                selectedToolIndices.Add(toolIndex);
                selectedToolNames.Add(toolName);
                toolButtons[toolIndex].GetComponent<Image>().color = selectedColor;
            }
        }

        UpdateConfirmButton();

    }

    void UpdateConfirmButton()
    {
        bool allToolsSelected = selectedToolIndices.Count == maxTools;

        // show confirmbutton only if max selected
        if (loadoutConfirmPanel != null)
        {
            loadoutConfirmPanel.SetActive(allToolsSelected);
        }

        // hide loadout panel when confirm panel shows
        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(!allToolsSelected);
        }
    }

    public void ShowConfirmPanel()
    {
        if (loadoutConfirmPanel != null)
            loadoutConfirmPanel.SetActive(true);
    }

    public List<string> GetSelectedToolNames()
    {
        return new List<string>(selectedToolNames);
    }

    public void ResetSelection()
    {
        // clear selections
        foreach (int index in selectedToolIndices)
        {
            toolButtons[index].GetComponent<Image>().color = unselectedColor;
        }

        selectedToolIndices.Clear();
        selectedToolNames.Clear();
        UpdateConfirmButton();
    }

    public void DeselectLastTool()
    {
        if (selectedToolIndices.Count > 0)
        {
            int lastIndex = selectedToolIndices[selectedToolIndices.Count - 1];
            selectedToolIndices.RemoveAt(selectedToolIndices.Count - 1);
            selectedToolNames.RemoveAt(selectedToolNames.Count - 1);
            toolButtons[lastIndex].GetComponent<Image>().color = unselectedColor;
            UpdateConfirmButton();
        }
    }
}
