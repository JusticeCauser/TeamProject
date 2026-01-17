using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class loadoutManager : MonoBehaviour
{
    [Header("Tool Buttons")]
    public Button[] toolButtons; //needs placeholders assigned, 10 gadgets so far

    [Header("UI")]
    public GameObject confirmLoadoutButton;
    public GameObject loadoutConfirmPanel;

    [Header("Selection Settings")]
    public int maxTools = 3; // maybe need to be default 4, but for now 3 for easier testing
    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;

    private List<string> selectedToolNames = new List<string>();
    private List<int> selectedToolIndices = new List<int>();

    private void Start()
    {
        // hide confirm buttons at starrt
        if (confirmLoadoutButton != null)
            confirmLoadoutButton.SetActive(false);

        // set up button listeners
        for(int i = 0; i < toolButtons.Length; i++)
        {
            int index = i; // capture index
            toolButtons[i].onClick.AddListener(() => ToggleTool(index));
        }
    }

    void ToggleTool(int toolIndex)
    {
        // get tool name from button text
        string toolName = toolButtons[toolIndex].GetComponentInChildren<TMP_Text>().text;

        if(selectedToolIndices.Contains(toolIndex))
        {
            //deselect
            selectedToolIndices.Remove(toolIndex);
            selectedToolNames.Remove(toolName);
            toolButtons[toolIndex].GetComponent<Image>().color = unselectedColor;
        }
        else
        {
            // select if under max
            if(selectedToolIndices.Count < maxTools)
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
        // show confirmbutton only if max selected
        if(confirmLoadoutButton != null)
        {
            confirmLoadoutButton.SetActive(selectedToolIndices.Count == maxTools);
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
        foreach(int index in selectedToolIndices)
        {
            toolButtons[index].GetComponent<Image>().color = unselectedColor;
        }

        selectedToolIndices.Clear();
        selectedToolNames.Clear();
        UpdateConfirmButton();
    }
}
