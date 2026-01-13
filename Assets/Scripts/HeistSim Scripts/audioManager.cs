using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [Header("-----Audio Sources-----")]
    [SerializeField] AudioSource ambientAudio;
    [SerializeField] AudioSource sfxAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    void Start()
    {
        setVolume();
    }

    // Update is called once per frame
    void Update()
    {
        //setVolume();
    }

    public void setVolume()
    {
        if(SettingsManager.instance == null) return;

        if (ambientAudio != null)
        {
            ambientAudio.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.ambientVolume;
        }

        if( sfxAudio != null)
            sfxAudio.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.sfxVolume;

    }
   

}
