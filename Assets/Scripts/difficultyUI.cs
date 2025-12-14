using UnityEngine;
using TMPro;

public class difficultyUI : MonoBehaviour
{
    [SerializeField] TMP_Text difficultyText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateDifficultyDisplay();
    }

    // left arrow button
    public void DecreaseDifficulty()
    {
        if (difficultyManager.instance == null) return;

        switch(difficultyManager.instance.currentDifficulty)
        {
            case difficultyManager.Difficulty.Easy:
                difficultyManager.instance.SetDifficulty(difficultyManager.Difficulty.Hard);
                break;
            case difficultyManager.Difficulty.Normal:
                difficultyManager.instance.SetDifficulty(difficultyManager.Difficulty.Easy);
                break;
            case difficultyManager.Difficulty.Hard:
                difficultyManager.instance.SetDifficulty(difficultyManager.Difficulty.Normal);
                break;
        }

        UpdateDifficultyDisplay();
    }

    // right arrow button 
    public void IncreaseDifficulty()
    {
        if (difficultyManager.instance == null) return;

        switch (difficultyManager.instance.currentDifficulty)
        {
            case difficultyManager.Difficulty.Easy:
                difficultyManager.instance.SetDifficulty(difficultyManager.Difficulty.Normal);
                break;
            case difficultyManager.Difficulty.Normal:
                difficultyManager.instance.SetDifficulty(difficultyManager.Difficulty.Hard);
                break;
            case difficultyManager.Difficulty.Hard:
                difficultyManager.instance.SetDifficulty(difficultyManager.Difficulty.Easy);
                break;
        }

        UpdateDifficultyDisplay();
    }

    void UpdateDifficultyDisplay()
    {
        if (difficultyText != null && difficultyManager.instance != null)
        {
            difficultyText.text = difficultyManager.instance.currentDifficulty.ToString().ToUpper();
        }
    }
}
