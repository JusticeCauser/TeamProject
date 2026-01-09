using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{

    [SerializeField] string asylumScene = "LoadingSceneAsylum";
    [SerializeField] string mansionScene = "LoadingSceneManison";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void loadSceneAsylum()
    {
        SceneManager.LoadScene(asylumScene);
    }

    public void loadSceneMansion()
    {
        SceneManager.LoadScene(mansionScene);
    }
}
