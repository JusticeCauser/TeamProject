using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] Image fade;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
     void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoad;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoad;
    }

    void onSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(fade != null)
            fade.color = new Color(0, 0, 0, 0);
    }
    public IEnumerator screenFadeToBlack() 
    {
        float fadeTime = 0f;

        while(fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;
            float fadeColor = (fadeTime / fadeDuration);    
            fade.color = new Color(0, 0, 0, fadeColor);
            yield return null;
        }
        fade.color = new Color(0, 0, 0, 1);
    }
    
}
