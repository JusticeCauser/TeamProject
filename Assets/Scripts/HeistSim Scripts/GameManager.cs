using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum fail{ captured, heatTimeExpired}
    private fail failReason;
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
    [SerializeField] TMP_Text timerTextWin;
    [SerializeField] TMP_Text itemValueText;
    [SerializeField] TMP_Text timerTextFail;
    [SerializeField] TMP_Text failText;



    float timeScaleOrig;

    float startTimer;
    float endTimer;

    bool timerOn;

    string sceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
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

        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");

        if (player != null) //bc player doesnt exist in introscene
        {
            playerScript = player.GetComponent<PlayerController>();
        }

    }
    void Start() //if statements shouldnt be needed when stable
    {//could create function for this to look cleaner in start
        if (wQuitButton != null)
            wQuitButton.onClick.AddListener(quitToLobby);

        if (lQuitButton != null)
            lQuitButton.onClick.AddListener(quitToLobby);

        if (retryButton != null)
            retryButton.onClick.AddListener(retry);

        sceneName = SceneManager.GetActiveScene().name; //timer may fail here to the end of if statement due to start runtime
        if (sceneName == "Asylum" || sceneName == "Mansion")
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
    private void OnEnable() //fixes scene issue with settings
    {
        SceneManager.sceneLoaded += onSceneLoad;
    }

    private void OnDisable() //fixes scene issue with settings
    {
        SceneManager.sceneLoaded -= onSceneLoad;
    }
    private void onSceneLoad(Scene scene, LoadSceneMode mode) // fixes issue where after quitting to lobby and loading back in, settings does not auto open
    {
        if (menuWin != null)
            menuWin.SetActive(false);

        if(menuLose != null)
            menuLose.SetActive(false);

        Time.timeScale = timeScaleOrig;
    }
    public void missionComplete()
    { //completing 
        if (timerOn)
        {
            endTimer = Time.time - startTimer;

            int min = Mathf.FloorToInt(endTimer / 60f);
            int sec = Mathf.FloorToInt(endTimer % 60f);

            timerTextWin.text = "Completed in: " + string.Format("{0:00}:{1:00}", min, sec);

        }
        if (itemValueText != null)
            itemValueText.text = "Total value collected: $" + playerScript.totalValue;

        if (menuWin != null)
        {
            menuWin.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void missionFail(fail reason)
    { //capture, if not out in 60 seconds from HEAT timer

        failReason = reason;
        if (timerOn)
        {
            endTimer = Time.time - startTimer;

            int min = Mathf.FloorToInt(endTimer / 60f);
            int sec = Mathf.FloorToInt(endTimer % 60f);

            timerTextFail.text = "Time Survived: " + string.Format("{0:00}:{1:00}", min, sec);

        }
        if (menuLose != null)
        {
            menuLose.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
       
            missionFailReason();
    }
    void missionFailReason()
    {
        if (failText == null)
            return;

        switch (failReason)
        {
            case fail.captured:
                failText.text = "Why'd you get caught?";
                break;

            case fail.heatTimeExpired:
                failText.text = "You didn't make it out in time. Backup arrived.";
                break;
        }
           
    }
    public void quitToLobby()
    {
        SceneManager.LoadScene("theHub");
    }
    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //string map = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(map);
    }
}