using UnityEngine;

public class GuardHeatDetection : MonoBehaviour
{
    public float heatPerSecond = 10f;
    public float instantHeat = 15f;

    public void OnPlayerSeen()
    {
        HeatManager.Instance.AddHeat(instantHeat);
    }

    public void WhilePlayerVisible()
    {
        HeatManager.Instance.AddHeat(heatPerSecond * Time.deltaTime);
    }

    public void OnNoiseHeard(float noiseLevel)
    {
        HeatManager.Instance.AddHeat(noiseLevel);
    }
}
