using UnityEngine;

public class GuardHeatListener : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HeatManager.Instance.OnHeatStateChanged += OnHeatChanged;
    }

    void OnHeatChanged(HeatState state)
    {
        switch (state)
        {
            case HeatState.Suspicious:
                Investigate();
                break;
            case HeatState.Searching:
                AlertNearbyGuards();
                BeginSearch();
                break;
            case HeatState.Hunt:
                BeginHunt();
                AttemptBackupCall();
                break;
            case HeatState.Emergency:
                LockDown();
                break;
        }
    }

    void Investigate() { }
    void AlertNearbyGuards() { }
    void BeginSearch() { }
    void BeginHunt() { }
    void AttemptBackupCall() { }
    void LockDown() { }
}
