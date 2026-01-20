using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeatUI : MonoBehaviour
{
    private static HeatUI instance;

    [Header("References")]
    public HeatManager heatManager;
    public Image heatFill;
    public TMP_Text heatStateText;
    public TMP_Text heatPercentText;
    public TMP_Text emergencyTimerText;

    [Header("Behavior")]
    public bool showPercent = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!heatManager)
            heatManager = HeatManager.Instance;

        if (heatManager != null)
            heatManager.OnHeatStateChanged += OnHeatStateChanged;

        RefreshAll();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (heatManager != null)
            heatManager.OnHeatStateChanged -= OnHeatStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (!heatManager)
            return;

        float pct01 = heatManager.heat / heatManager.maxHeat;
        heatFill.fillAmount = pct01;

        if (showPercent && heatPercentText)
            heatPercentText.text = Mathf.RoundToInt(pct01 * 100f) + "%";

        if (emergencyTimerText)
        {
            if (heatManager.currentState == HeatState.Emergency)
            {
                emergencyTimerText.gameObject.SetActive(true);
                emergencyTimerText.text = "ESCAPE: " + Mathf.CeilToInt(heatManager.GetEmergencyTimeRemaining()) + "s";
            }
            else
            {
                emergencyTimerText.gameObject.SetActive(false);
            }
        }
    }

    void OnHeatStateChanged(HeatState state)
    {
        RefreshState(state);
    }

    void RefreshAll()
    {
        if (!heatManager)
            return;
        RefreshState(heatManager.currentState);
    }

    void RefreshState(HeatState state)
    {
        if (!heatStateText)
            return;

        switch (state)
        {
            case HeatState.Suspicious:
                heatStateText.text = "SUSPICIOUS";
                break;
            case HeatState.Searching:
                heatStateText.text = "SEARCHING";
                break;
            case HeatState.Hunt:
                heatStateText.text = "HUNT";
                break;
            case HeatState.Emergency:
                heatStateText.text = "EMERGENCY";
                break;
            default:
                heatStateText.text = "CALM";
                break;
        }
    }
}
