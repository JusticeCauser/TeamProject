using UnityEngine;

public class SaveState : MonoBehaviour
{
    public static SaveState Instance;

    public string lastScene;
    public string lastSpawnId = "Default";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
