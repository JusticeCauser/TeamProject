using UnityEngine;

public class GlassShardSound : MonoBehaviour
{
    public AudioClip crunchSound;
    private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            source.PlayOneShot(crunchSound);
        }
    }

}
