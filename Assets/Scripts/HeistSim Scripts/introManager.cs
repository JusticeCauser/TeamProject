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
        SceneManager.LoadScene(theHub);

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
