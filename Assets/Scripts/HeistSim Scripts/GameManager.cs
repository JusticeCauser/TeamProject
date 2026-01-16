using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public PlayerController playerScript;

    [Header("---Menus---")]
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("---Buttons---")]
    [SerializeField] Button wQuitButton;
    [SerializeField] Button lQuitButton;
    [SerializeField] Button retryButton;

    [Header("---Timer---")]
    [SerializeField] TMP_Text timerText;
    float timeScaleOrig;

    float startTimer;
    float endTimer;

    bool timerOn;

    string sceneName;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        
        if(player != null) //bc player doesnt exist in introscene
        {
            playerScript = player.GetComponent<PlayerController>();
        }
       
    }
    void Start() //if statements shouldnt be needed when stable
    {//could create function for this to look cleaner in start
        if (wQuitButton != null)
            wQuitButton.onClick.AddListener(quitToLobby);

        if(lQuitButton != null)
            lQuitButton.onClick.AddListener(quitToLobby);

        if (retryButton != null)
            retryButton.onClick.AddListener(retry);

        sceneName = SceneManager.GetActiveScene().name; //timer may fail here to the end of if statement due to start runtime
        if(sceneName == "Asylum" || sceneName == "Mansion")
        {
            startTimer = Time.time;
            timerOn = true;
        }
        else
        {
            timerOn = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void missionComplete()
    {
        if (timerOn)
        {
            endTimer = Time.time - startTimer;

            int min = Mathf.FloorToInt(endTimer / 60f);
            int sec = Mathf.FloorToInt(endTimer % 60f);

            timerText.text = "Completed in: " + string.Format("{0:00}:{1:00}", min, sec);
        }

        if(menuWin != null)
        {
            menuWin.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void missionFail()
    {
        if(menuLose != null)
        {
            menuLose.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void quitToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        //string map = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(map);
    }
}
