using UnityEngine;
public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [Header("Rebindable Keys")]
    [SerializeField] KeyCode defaultInteractKey = KeyCode.E;
    KeyCode interactKey;
    void Awake() //persist across scenes
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            interactKey = PlayerPrefs.HasKey("InteractKey") ? (KeyCode)PlayerPrefs.GetInt("InteractKey") : defaultInteractKey; //cleaner than if statement
        }
        else
        {
            Destroy(gameObject);

        }
    }
  
    public bool interactKeyPressed()
    {
        return Input.GetKeyDown(interactKey);
    }
    public KeyCode getinteractKey() //getter for current key
    {
        return interactKey;

    }
    public void setInteractKey(KeyCode key) //setter- sets new key and saves players choice
    {
        interactKey = key;
        PlayerPrefs.SetInt("InteractKey", (int)interactKey);
        PlayerPrefs.Save();
    }
    public void resetToDefault()
    {
        setInteractKey(defaultInteractKey);
    }
}