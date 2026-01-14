using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    public enum ambientType { none, intro, lobby, asylum, mansion}
    ambientType curr = ambientType.none;

    [Header("---Audio Settings---")]
    public float masterVolume = 1f;
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;

    [Header("---Audio Sources---")]
    [SerializeField] AudioSource ambientAudio;
    [SerializeField] AudioSource sfxAudio;

    [Header("----Audio Clips---")]
    [SerializeField] AudioClip introClip;
    [SerializeField] AudioClip lobbyClip;
    [SerializeField] AudioClip asylumClip;
    [SerializeField] AudioClip mansionClip;


    [Header("---SFX Clips---")]
    [SerializeField] AudioClip bark;
    [SerializeField] AudioClip footsteps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnLoad;

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
        
    }
    public void setVolume()
    {
        if (SettingsManager.instance == null) return;
        if (ambientAudio != null)
        
            ambientAudio.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.ambientVolume;
        

        if( sfxAudio != null)
            sfxAudio.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.sfxVolume;

    }
   
    public void ambientAudioType(ambientType type) //changes the audio pertaining to the scene
    {
        //if (ambientAudio == null) return;
        if(type == curr)
            return;

        curr = type;
        
        
        switch (type)
        {

            case ambientType.none:
                return;

            case ambientType.intro:
                ambientAudio.clip = introClip;
                break;

                case ambientType.lobby:
                    ambientAudio.clip = lobbyClip;
                break;

            case ambientType.asylum:
                ambientAudio.clip = asylumClip;
                break;

            case ambientType.mansion:
                ambientAudio.clip = mansionClip;
                break;

        }

        ambientAudio.loop = true;
        setVolume();
        ambientAudio.Play();
         
    }

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
       
        switch (scene.name)
        {
            case "IntroScene":
                ambientAudioType(ambientType.intro);
                break;

            case "Lobby":
                ambientAudioType(ambientType.lobby);
                break;

            case "LoadingIntroAsylum":
            case "Asylum":
                ambientAudioType(ambientType.asylum);
                break;

            case "LoadingIntroMansion":
            case "Mansion":
                ambientAudioType(ambientType.mansion);
                break;

            default:
                ambientAudioType(ambientType.none);
                break;

        }

    }

}
