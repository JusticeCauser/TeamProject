using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager instance;

    [Header("Rebindable Keys")]
    [SerializeField] KeyCode defaultInteractKey = KeyCode.E;

    KeyCode interactKey;

    bool isRebound;
    void Awake() //persist across scenes
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void getinteractKey(KeyCode key) //getter
    {
        interactKey = key;
       
    }

    public void setInteractKey(KeyCode key) //setter
    {
        interactKey = key;
        PlayerPrefs.Save();
    }
    public void resetToDefault()
    {
        setInteractKey(defaultInteractKey);
    }
}
