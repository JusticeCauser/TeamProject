using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class introButtons : MonoBehaviour
{

    [SerializeField] string theHub = "theHub";
    
    

    private void Start()
    {
        
    }
    public void Play()
    {
        if (audioManager.instance != null)
            audioManager.instance.audioFadeOnTransition();

        StartCoroutine(FadeManager.instance.transition(theHub));
        //SceneManager.LoadScene(theHub);

    }
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
}
