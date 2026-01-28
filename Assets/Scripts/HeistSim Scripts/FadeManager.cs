using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = .5f;
    public Image fade;

    bool isTransition;
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
        if (!isTransition)
            return;

        if(fade != null)
            fade.color = new Color(0, 0, 0, 1);
        StartCoroutine(screenFadeFromBlack());
    }
    public IEnumerator screenFadeToBlack() 
    {
        float fadeTime = 0f;

        fade.color = new Color(0, 0, 0, 0f);
        yield return null;

        while(fadeTime < fadeDuration)
        {
            fadeTime += Time.unscaledDeltaTime;
            float fadeColor = (fadeTime / fadeDuration);    
            fade.color = new Color(0, 0, 0, fadeColor);
            yield return null;
        }
        fade.color = new Color(0, 0, 0, 1f);
    }
    public IEnumerator screenFadeFromBlack()
    {
        float fadeTime = 0f;

        while(fadeTime < fadeDuration)
        {
            fadeTime += Time.unscaledDeltaTime;
            float fadeColor = 1f - (fadeTime / fadeDuration); //fading from black
            fade.color = new Color(0, 0, 0, fadeColor);
            yield return null;
        }
        fade.color = new Color(0, 0, 0, 0f);
    }

    public IEnumerator transition(string scene)
    {
        isTransition = true;

        if (audioManager.instance != null)
            audioManager.instance.audioFadeOnTransition();

        fade.enabled = true;
        fade.color = new Color(0, 0, 0, 0.01f);

        yield return StartCoroutine(screenFadeToBlack());
        SceneManager.LoadScene(scene);
    }
}
