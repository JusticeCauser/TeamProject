using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    public bool ElevatorUnlocked { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ElevatorUnlocked = PlayerPrefs.GetInt("ElevatorUnlocked", 0) == 1;

#if UNITY_EDITOR
        PlayerPrefs.DeleteKey("ElevatorUnlocked");
        ElevatorUnlocked = false;
#endif
    }

    public void SetElevatorUnlocked(bool value)
    {
        ElevatorUnlocked = value;
        PlayerPrefs.SetInt("ElevatorUnlocked", value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
