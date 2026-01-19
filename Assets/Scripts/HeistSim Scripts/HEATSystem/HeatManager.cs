using UnityEngine;
using System;
using System.Runtime.ExceptionServices;
using UnityEngine.InputSystem.XR.Haptics;

public class HeatManager : MonoBehaviour
{
    [Header("Heat Settings")]
    public float heat = 0f;
    public float maxHeat = 100f;
    public float maxHeatReached;

    [Header("Decay")]
    public float heatDecayRate = 5f;
    public float decayDelay = 3f;

    [Header("Emergency")]
    public float escapeTime = 60f;

    private float lastHeatGainTime;
    private float emergencyTimer;

    public HeatState currentState {  get; private set; }

    public static HeatManager Instance;

    public event Action<HeatState> OnHeatStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDecay();
        HandleState();
        HandleEmergencyTimer();
    }

    public void AddHeat(float amount)
    {
        heat = Mathf.Clamp(heat + amount, 0, maxHeat);
        lastHeatGainTime = Time.time;

        if(heat > maxHeatReached)
            maxHeatReached = heat;
    }

    public void ReducedHeat(float amount)
    {
        heat = Mathf.Clamp(heat - amount, 0, maxHeat);
    }

    void HandleDecay()
    {
        if (Time.time > lastHeatGainTime + decayDelay && heat > 0)
        {
            heat -= heatDecayRate * Time.deltaTime;
            heat = Mathf.Clamp(heat, 0, maxHeat);
        }
    }

    void HandleState()
    {
        HeatState newState = GetStateFromHeat();

        if (newState != currentState)
        {
            currentState = newState;
            OnHeatStateChanged?.Invoke(currentState);

            if (currentState == HeatState.Emergency)
            {
                emergencyTimer = escapeTime;
            }
        }
    }

    HeatState GetStateFromHeat()
    {
        if (heat < 25f)
            return HeatState.Suspicious;
        if (heat < 50f)
            return HeatState.Searching;
        if (heat < 75f)
            return HeatState.Hunt;
        return HeatState.Emergency;
    }

    void HandleEmergencyTimer()
    {
        if (currentState != HeatState.Emergency)
            return;

        emergencyTimer -= Time.deltaTime;

        if (emergencyTimer <= 0f)
        {
            FailMission();
        }
    }

    void FailMission()
    {
        // Hook into fail screen / reload logic
    }

    public float GetEmergencyTimeRemaining()
    {
        return emergencyTimer;
    }
}
