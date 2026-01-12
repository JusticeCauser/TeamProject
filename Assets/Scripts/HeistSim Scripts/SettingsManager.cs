using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager instance;

    [Header("-----Audio Settings-----")]
    [SerializeField] Slider masterSlide;
    [SerializeField] Slider ambientSlide;
    [SerializeField] Slider sfxSlide;
    public float masterVolume = 1f;
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;


    private void Start()
    {
        if(instance == null)
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
        }
    }

    public void setMasterVolume(float volume)
    {
        masterVolume = volume;

        PlayerPrefs.SetFloat("Master Volume", volume); //set to what player chose
        PlayerPrefs.Save(); 
    }

    public void setAmbientVolume(float volume)
    {
        ambientVolume = volume;

        PlayerPrefs.SetFloat("Ambient Volume", volume); //set to what player chose
        PlayerPrefs.Save();
    }

    public void setSFXVolume(float volume)
    {
        sfxVolume = volume;

        PlayerPrefs.SetFloat("SFX Volume", volume); //set to what player chose
        PlayerPrefs.Save();
    }
}
