using UnityEngine;
using UnityEngine.UI;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    public enum ambientType { none, asylum, mansion}
    ambientType curr = ambientType.none;

    [Header("-----Audio Sources-----")]
    [SerializeField] AudioSource ambientAudio;
    [SerializeField] AudioSource sfxAudio;

    [Header("------Audio Clips-----")]
    [SerializeField] AudioClip asylumClip;
    [SerializeField] AudioClip mansionClip;

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
        

        if (ambientAudio != null)
        {
            ambientAudio.volume = masterVolume * ambientVolume;
        }

        if( sfxAudio != null)
            sfxAudio.volume = masterVolume * sfxVolume;

    }
   
    public void ambientAudioType(ambientType type) //changes the audio pertaining to the scene
    {
        if(type == curr)
            return;

        curr = type;
        
        if(type == ambientType.none)
        {
            ambientAudio.Stop();
            return;
        }
        switch (type)
        {
            case ambientType.asylum:
                ambientAudio.clip = asylumClip;
                break;

            case ambientType.mansion:
                ambientAudio.clip = mansionClip;
                break;

            case ambientType.none:
                ambientAudio.Stop();
                return;

        }

        ambientAudio.loop = true; 
        ambientAudio.Play();
        setVolume(); 
    }

}
