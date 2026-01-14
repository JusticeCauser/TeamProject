using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager instance;

    [Header("Settings UI")]
    [SerializeField] GameObject settingsUI;
    [SerializeField] Button backButton;

    [Header("-----Audio Sliders-----")]
    [SerializeField] Slider masterSlide;
    [SerializeField] Slider ambientSlide;
    [SerializeField] Slider sfxSlide;


    public float masterVolume = 1f;
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;


    private void Awake() 
    {
        //Debug.Log("settingsmanage awake " +gameObject.name);
        //Debug.Log("instance: " + instance);
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

           // Debug.Log("settings manager alive");
          
        }
        else
        {
           // Debug.Log("settings manager dead");
            Destroy(gameObject);
            return;
        }
    }
    //private void OnEnable()
    //{
    //    Debug.Log("settingsmanager enable");
    //}

    //private void OnDisable()
    //{
    //    Debug.Log("settingsmanager disable");
    //}
    //private void OnDestroy()
    //{
    //    Debug.Log("destroyed");
    //}

    

    private void Start()
    {
        if(masterSlide != null) masterSlide.SetValueWithoutNotify(masterVolume);
        masterSlide.onValueChanged.AddListener(setMasterVolume);

        if (ambientSlide != null) ambientSlide.SetValueWithoutNotify(ambientVolume);
        ambientSlide.onValueChanged.AddListener(setAmbientVolume);

        if (sfxSlide != null) sfxSlide.SetValueWithoutNotify(sfxVolume);
        sfxSlide.onValueChanged.AddListener(setSFXVolume);

        if (settingsUI != null)
            settingsUI.SetActive(false);
        //else
        //    Debug.LogWarning("Settings ui not assigned");
        if (backButton != null)
            backButton.onClick.AddListener(closeSettings);
        //else
        //    Debug.LogWarning("backbutton not assigned");
        
    }

    public void openSettings()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        //    Debug.Log("settings ui opened");
        //}
        //else
        //    Debug.LogWarning("settings ui reference missing");
    }
    public void closeSettings()
    {
        if (settingsUI != null)
            settingsUI.SetActive(false);
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

   
}
