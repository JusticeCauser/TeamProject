using System.Collections;
using UnityEngine;

public class VehicleLightController : MonoBehaviour
{
    [Header("Lights")]
    public Light[] headLights;
    public Light[] brakeLights;

    [Header("Timing")]
    public float brakeOnDuringIdleSeconds = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    IEnumerator BrakeRoutine()
    {
        SetLights(brakeLights, true);
        yield return new WaitForSeconds(brakeOnDuringIdleSeconds);

        SetLights(brakeLights, false);
    }

    void SetLights(Light[] lights, bool on)
    {
        if (lights == null)
            return;
        foreach (var l in lights)
            if (l) l.enabled = on;
    }
}
