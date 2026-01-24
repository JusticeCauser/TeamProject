using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager instance;

    [Header("---Keybind Options---")]
    [SerializeField] TMP_Text interactText;
    [SerializeField] Button keyBindButton;
    [SerializeField] Button resetButton;

    bool rebindKey;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(keyBindButton != null)
        {
            keyBindButton.onClick.AddListener(playInteractButtonSound);
            keyBindButton.onClick.AddListener(rebind);
        }

        if(resetButton != null)
        {
            resetButton.onClick.AddListener(playInteractButtonSound);
            resetButton.onClick.AddListener(resetKeybind);
        }

        updateInteractText();
    }
    // Update is called once per frame
    void Update() //allowing player to set keybind 
    {
        if (rebindKey)
            setRebind();
    }
    void rebind()
    {
        rebindKey = true;
    }
    void setRebind() 
    {
        if (interactText != null)
            interactText.text = "Select new interact key (ESC to cancel)";

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            rebindKey = false;
            updateInteractText();
            return;
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                InputManager.instance.setInteractKey(key);
                rebindKey = false;
                updateInteractText();
                return;
            }
        }
    }
    public void updateInteractText() //making public so the text will also show restored to default
    {
        if(interactText != null) //displays what the keybind is set to
            interactText.text = "Interact: " + InputManager.instance.getinteractKey().ToString();
    }
    void resetKeybind()
    {
        InputManager.instance.resetToDefault();
        updateInteractText();
    }
    void playInteractButtonSound() //grabbing the same button sound from audio manager used on the settings menu
    {
        if (audioManager.instance != null)
            audioManager.instance.playButtonSound();
    }
}
