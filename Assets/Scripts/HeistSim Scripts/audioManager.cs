using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [Header("-----Audio Sources-----")]
    [SerializeField] AudioSource ambientAudio;
    [SerializeField] AudioSource sfxAudio;

    [Header("---Audio Settings---")]
    public float masterVolume = 1f;
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);


            masterVolume = PlayerPrefs.GetFloat("Master Volume", 1f); //load default volume or saved
            ambientVolume = PlayerPrefs.GetFloat("Ambient Volume", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFX Volume", 1f);
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
    public void setAmbientVolume(float volume)
    {
        ambientVolume = volume;

        PlayerPrefs.SetFloat("Ambient Volume", volume); //set to what player chose
        PlayerPrefs.Save();


        setVolume();

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
