using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    public enum ambientType { none, intro, lobby, asylum, mansion }
    private ambientType curr = ambientType.none;
    private string _sceneLoad = "";

    [Header("Fade Settings")]
    public float duration = .5f;
    

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
    [SerializeField] AudioClip missionCompleteClip;
    [SerializeField] AudioClip missionFailClip;

    [Header("---SFX Clips---")]
    [SerializeField] AudioClip bark;
    [SerializeField] AudioClip footsteps;
    [SerializeField] AudioClip menuButtonClicked;
    [SerializeField] AudioClip slider;
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
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoad;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoad;
    }
    void Start()
    {
        setVolume();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playButtonSound()
    {
        if (sfxAudio != null && menuButtonClicked != null)
            sfxAudio.PlayOneShot(menuButtonClicked);
    }
    public void playSliderSound(float value)
    {
        if(sfxAudio != null && slider  != null)
            sfxAudio.PlayOneShot(slider);
    }
    public void playMissionCompleteSound()
    {
        if(sfxAudio  != null && missionCompleteClip != null)
            sfxAudio.PlayOneShot(missionCompleteClip);
    }
    public void playMissionFailSound()
    {
        if(sfxAudio != null && missionFailClip != null)
            sfxAudio.PlayOneShot(missionFailClip);
    }
    public void setVolume()
    {
        if (SettingsManager.instance == null) return;
        if (ambientAudio != null)

            ambientAudio.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.ambientVolume;


        if (sfxAudio != null)
            sfxAudio.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.sfxVolume;

    }

    public void ambientAudioType(ambientType type) //changes the audio pertaining to the scene
    {
        if (ambientAudio == null) return;

        curr = type;


        switch (type)
        {

            case ambientType.none:
                ambientAudio.Stop();
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
        StartCoroutine(fadeAudios());
    }

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _sceneLoad) return;

        _sceneLoad = scene.name;

        switch (scene.name)
        {
            case "IntroScene":
                ambientAudioType(ambientType.intro);
                break;

            case "theHub":
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
    public void audioFadeOnTransition()
    {
        if(ambientAudio == null) 
            return;

        StopCoroutine("fadeAudios");
        StopCoroutine("fadeAudioIn");
        StartCoroutine(fadeAudioOut());
    }
    IEnumerator fadeAudioOut()
    {
        float timer = 0f;
        float startVol = ambientAudio.volume;

        if(ambientAudio == null)
            yield break;

        while(timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            ambientAudio.volume = Mathf.Lerp(startVol, 0, timer /  duration);
            yield return null;
        }

        ambientAudio.Stop();
    }

    IEnumerator fadeAudioIn()
    {
        float timer = 0f;
        float setVolume = SettingsManager.instance.masterVolume * SettingsManager.instance.ambientVolume;

        ambientAudio.volume = 0f;
        ambientAudio.Play();

        if(ambientAudio == null)
            yield break;

        while(timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            ambientAudio.volume = Mathf.Lerp(0f, setVolume, timer / duration);
            yield return null;
        }
    }

    IEnumerator fadeAudios()
    {
        yield return StartCoroutine(fadeAudioOut());
        yield return StartCoroutine(fadeAudioIn());
    }

}