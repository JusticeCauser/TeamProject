using UnityEngine;

public class NoiseSpikeTrigger : MonoBehaviour
{
    public float spike = 40f;
    public string source = "Glass Step";
    public float cooldown = 0.5f;

    float nextTime;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextTime)
            return;
        if (!other.CompareTag("Player"))
            return;

        nextTime = Time.time + cooldown;
        NoiseManager.Instance.AddSpike(spike, source);
    }
}
