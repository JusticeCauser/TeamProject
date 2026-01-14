using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager instance;

    [Header("-----Audio Sliders-----")]
    [SerializeField] Slider masterSlide;
    [SerializeField] Slider ambientSlide;
    [SerializeField] Slider sfxSlide;


    public float masterVolume = 1f;
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;


    private void Awake() //sli ders will not 
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            masterVolume = 1f;
            ambientVolume = 1f;
            sfxVolume = 1f;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if(masterSlide != null) masterSlide.SetValueWithoutNotify(masterVolume);
        if(ambientSlide != null) ambientSlide.SetValueWithoutNotify(ambientVolume);
        if(sfxSlide != null) sfxSlide.SetValueWithoutNotify(sfxVolume);

        

        masterSlide.onValueChanged.AddListener(setMasterVolume);
        ambientSlide.onValueChanged.AddListener(setAmbientVolume);
        sfxSlide.onValueChanged.AddListener(setSFXVolume);
       
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
