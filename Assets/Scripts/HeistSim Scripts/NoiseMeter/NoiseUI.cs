using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoiseUI : MonoBehaviour
{
    public NoiseManager noiseManager;
    public Image noiseFill;
    public TMP_Text labelText;

    [Header("Optional Color Thresholds")]
    public bool useColors = false;
    public float quietMax = 30f;
    public float noticeableMax = 60f;

    public CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!noiseManager)
            noiseManager = NoiseManager.Instance;
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!noiseManager)
            return;

        float n01 = noiseManager.noise / 100f;

        if (noiseFill)
            noiseFill.fillAmount = n01;

        if (labelText)
        {
            float n = noiseManager.noise;
            if (n <= quietMax)
                labelText.text = "Quiet";
            else if (n <= noticeableMax)
                labelText.text = "Noticeable";
            else
                labelText.text = "Loud";
        }

        if (canvasGroup)
            canvasGroup.alpha = noiseManager.HudAlpha01;
    }
}
