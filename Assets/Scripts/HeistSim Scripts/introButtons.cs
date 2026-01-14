using UnityEngine;
using UnityEngine.SceneManagement;

public class introButtons : MonoBehaviour
{

    [SerializeField] string lobby = "Lobby";
    
    

    private void Start()
    {
        
    }
    public void Play()
    {
        SceneManager.LoadScene(lobby);

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
