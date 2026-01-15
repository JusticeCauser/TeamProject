using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager instance;
    
    [SerializeField] string lobby = "lobby";
    [SerializeField] string title = "IntroScene";

    [Header("Settings UI")]
    [SerializeField] GameObject settingsUI;
    [SerializeField] Button backButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button quitGameButton;

    [Header("-----Audio Sliders-----")]
    [SerializeField] Slider masterSlide;
    [SerializeField] Slider ambientSlide;
    [SerializeField] Slider sfxSlide;


    public float masterVolume = 1f;
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;

    float timeScaleOrig;

    public bool isActive = false;

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

    private void Start()
    {

        timeScaleOrig = Time.timeScale;

        if (masterSlide != null) masterSlide.SetValueWithoutNotify(masterVolume);
        masterSlide.onValueChanged.AddListener(setMasterVolume);

        if (ambientSlide != null) ambientSlide.SetValueWithoutNotify(ambientVolume);
        ambientSlide.onValueChanged.AddListener(setAmbientVolume);

        if (sfxSlide != null) sfxSlide.SetValueWithoutNotify(sfxVolume);
        sfxSlide.onValueChanged.AddListener(setSFXVolume);

        if (settingsUI != null)
            settingsUI.SetActive(false);

        if (backButton != null)
            backButton.onClick.AddListener(closeSettings);


    }
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isActive)
            {
                closeSettings();
            }
            else
            {
                openSettings();
            }
        }
    }

    private void OnEnable() //fixes scene issue with settings
    {
        SceneManager.sceneLoaded += onSceneLoad;
    }

    private void OnDisable() //fixes scene issue with settings
    {
        SceneManager.sceneLoaded -= onSceneLoad;
    }
    private void onSceneLoad(Scene scene, LoadSceneMode mode) // fixes issue where after quitting to lobby and loading back in, settings does not auto open
    {
        if (settingsUI != null)
            settingsUI.SetActive(false);

        isActive = false;
       

    }
    public void openSettings()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
            isActive = true;
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }

    public void closeSettings()
    {
        if (settingsUI != null)
            settingsUI.SetActive(false);

        isActive = false;
        Time.timeScale = timeScaleOrig;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void setMasterVolume(float volume)
    {
        masterVolume = volume;

        PlayerPrefs.SetFloat("Master Volume", volume); //set to what player chose
        PlayerPrefs.Save();
        audioManager.instance.setVolume();

    }

    public void setAmbientVolume(float volume)
    {
        ambientVolume = volume;

        PlayerPrefs.SetFloat("Ambient Volume", volume); //set to what player chose
        PlayerPrefs.Save();

        audioManager.instance.setVolume();

    }

    public void setSFXVolume(float volume)
    {
        sfxVolume = volume;

        PlayerPrefs.SetFloat("SFX Volume", volume); //set to what player chose
        PlayerPrefs.Save();
        audioManager.instance.setVolume();

    }

    public void quitToLobby()
    {
        if (SceneManager.GetActiveScene().name == lobby) //if youre in lobby it just closes menu
        {
            closeSettings();
            return;
        }
            

        Time.timeScale = timeScaleOrig;

        if(settingsUI != null)
             settingsUI.SetActive(false);

        isActive = false;
        SceneManager.LoadScene(lobby);
    }

    public void quitToTitle()
    {
        Time.timeScale = timeScaleOrig;
        SceneManager.LoadScene(title);
    }
}