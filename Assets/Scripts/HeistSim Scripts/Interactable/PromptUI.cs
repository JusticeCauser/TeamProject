using TMPro;
using UnityEngine;

public class PromptUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text text;

    public void Show(string msg)
    {
        if (group)
            group.alpha = 1f;
        if (text)
            text.text = msg;
    }

    public void Hide()
    {
        if (group)
            group.alpha = 0f;
        if (text)
            text.text = "";
    }
}
