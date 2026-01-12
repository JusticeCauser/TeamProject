using UnityEngine;
using UnityEngine.SceneManagement;

public class introButtons : MonoBehaviour
{

    [SerializeField] string lobby = "Lobby";
    
    [SerializeField] GameObject introSettings;

    private void Start()
    {
        if(introSettings != null)
            introSettings.SetActive(false);
    }
    public void Play()
    {
        SceneManager.LoadScene(lobby);

    }
    public void openSettings()
    {
        if(introSettings != null) 
            introSettings.SetActive(true);
    }
    public void closeSettings()
    {
        if(introSettings != null) 
            introSettings.SetActive(false);
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
