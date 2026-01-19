using UnityEngine;

public class BreakableGlass : MonoBehaviour
{
    [Header("Break Settings")]
    public float breakDelay = 0.2f;
    public GameObject shatteredGlassPrefab;

    [Header("Audio")]
    public AudioClip crackSound;
    public AudioClip shatterSound;

    private AudioSource audioSource;
    private bool isBroken = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBroken)
            return;

        if (other.CompareTag("Player"))
        {
            if (crackSound)
                audioSource.PlayOneShot(crackSound);

            Invoke(nameof(BreakableGlass), breakDelay);
            isBroken = true;
        }
    }

    void BreakGlass()
    {
        if (shatterSound)
            audioSource.PlayOneShot(shatterSound);

        if (shatteredGlassPrefab)
            Instantiate(shatteredGlassPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
