using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum fail { captured, heatTimeExpired }
    private fail failReason;
    public static GameManager instance;

    public GameObject player;
    public PlayerController playerScript;

    [SerializeField] string currScene;

    [Header("---HUD---")]
    [SerializeField] GameObject heatUIRoot;

    [Header("---HUD Rules---")]
    [SerializeField] bool useHeatUiWhiteList = true;
    [SerializeField]
    string[] ScenesWithHeatUI =
    {
        "Asylum",
        "Mansion",
        "MansionVault"
    };

    [Header("---Menus---")]
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("---Buttons---")]
    [SerializeField] Button wQuitButton;
    [SerializeField] Button lQuitButton;
    [SerializeField] Button retryButton;

    [Header("---Text Fields---")]
    [SerializeField] TMP_Text timerTextWin;
    [SerializeField] TMP_Text itemValueText;
    //[SerializeField] TMP_Text itemValueTextFail;
    [SerializeField] TMP_Text timerTextFail;
    [SerializeField] TMP_Text failText;
    [SerializeField] TMP_Text maxHeatTextWin;
    [SerializeField] TMP_Text maxHeatTextFail;
    [SerializeField] TMP_Text objectivesBonusText;
    [SerializeField] TMP_Text objectivesMissionCompletedText;
    [SerializeField] TMP_Text objectivesMissionFailedText;

    float timeScaleOrig;

    float startTimer;
    float endTimer;

    bool timerOn;

    string sceneName;

    public AlertSystem alertSys;
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
        //playerScript = player.GetComponent<PlayerController>();

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

        bool showHeat = ShouldShowHeatUI(scene.name);

        if (heatUIRoot != null)
            heatUIRoot.SetActive(showHeat);

        Time.timeScale = timeScaleOrig;

        player = GameObject.FindWithTag("Player");
        if (player != null)
            playerScript = player.GetComponent<PlayerController>();

        SetInteractionLocked(false);
    }
    public void missionComplete()
    { //completing 

        StartCoroutine(missionCompleteWithScreenFade());
    }
    IEnumerator missionCompleteWithScreenFade()
    {
        if (FadeManager.instance != null)
            yield return StartCoroutine(FadeManager.instance.screenFadeToBlack());

        if (audioManager.instance != null)
            audioManager.instance.playMissionCompleteSound();

        if (timerOn)
        {
            endTimer = Time.time - startTimer;

            int min = Mathf.FloorToInt(endTimer / 60f);
            int sec = Mathf.FloorToInt(endTimer % 60f);

            timerTextWin.text = "Completed in: " + string.Format("{0:00}:{1:00}", min, sec);

        }

        if (itemValueText != null && playerScript != null)
            itemValueText.text = "Total value collected: $" + playerScript.totalValue;

        if (objectivesBonusText != null && ObjectiveManager.instance != null)
        {
            int bonus = ObjectiveManager.instance.GetTotalMoneyBonus();

            if (bonus > 0)
                objectivesBonusText.text = "Objective Bonus: $" + bonus + "\n";
            else
                objectivesBonusText.text = "Objective Bonus: $0";
        }

        if (maxHeatTextWin != null && HeatManager.Instance != null) //shows only whole percentage, no decimals
            maxHeatTextWin.text = "Max Heat: " + HeatManager.Instance.maxHeatReached.ToString("F0") + "%";

        if (objectivesMissionCompletedText != null && ObjectiveManager.instance != null)
        {
            ObjectiveManager.instance.objectivesCompleted();
            ObjectiveManager.instance.showObjectivesCompleted();
            objectivesMissionCompletedText.text = ObjectiveManager.instance.objectivesCompleteText;
        }

        if (menuWin != null)
        {
            SetInteractionLocked(true);

            if (heatUIRoot != null)
            {
                heatUIRoot.SetActive(false);
            }
            menuWin.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (InventoryManager.instance != null && InventoryManager.instance.inventoryUI != null)
            InventoryManager.instance.inventoryUI.SetActive(false);
    }

    public void missionFail(fail reason)
    { //capture, if not out in 60 seconds from HEAT timer
        Debug.Log("Mission fail");

        StartCoroutine(missionFailWithScreenFade(reason));
        
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
    IEnumerator missionFailWithScreenFade(fail reason)
    {
        if (FadeManager.instance != null)
            yield return StartCoroutine(FadeManager.instance.screenFadeToBlack());

        if (audioManager.instance != null)
            audioManager.instance.playMissionFailSound();

        failReason = reason;

        if (timerOn)
        {
            endTimer = Time.time - startTimer;

            int min = Mathf.FloorToInt(endTimer / 60f);
            int sec = Mathf.FloorToInt(endTimer % 60f);

            timerTextFail.text = "Time Survived: " + string.Format("{0:00}:{1:00}", min, sec);

        }
        if (maxHeatTextFail != null && HeatManager.Instance != null) //shows only whole percentage, no decimals
            maxHeatTextFail.text = "Max Heat: " + HeatManager.Instance.maxHeatReached.ToString("F0") + "%";

        //if (itemValueTextFail != null && playerScript != null)
        //    itemValueTextFail.text = "Total value collected: $" + playerScript.totalValue;

        if (objectivesMissionCompletedText != null && ObjectiveManager.instance != null)
        {
            ObjectiveManager.instance.objectivesCompleted();
            ObjectiveManager.instance.showObjectivesCompleted();
            objectivesMissionFailedText.text = ObjectiveManager.instance.objectivesCompleteText;
        }

        if (menuLose != null)
        {
            SetInteractionLocked(true);

            if (heatUIRoot != null)
                heatUIRoot.SetActive(false);
            menuLose.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (InventoryManager.instance != null && InventoryManager.instance.inventoryUI != null)
            InventoryManager.instance.inventoryUI.SetActive(false);

        missionFailReason();
    }
    public void quitToLobby()
    {
        SceneManager.LoadScene("theHub");
    }
    public void retry()
    {
        if (ObjectiveManager.instance != null)
            ObjectiveManager.instance.resetObjectives();

        currScene = SceneManager.GetActiveScene().name;

        if (currScene == "Asylum")
            SceneManager.LoadScene("LoadingIntroAsylum");

        else if (currScene == "Mansion")
            SceneManager.LoadScene("LoadingIntroMansion");

        else
            SceneManager.LoadScene(currScene);

        //string map = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(map);
    }

    bool ShouldShowHeatUI(string sceneName)
    {
        if (!useHeatUiWhiteList)
            return true;

        if (ScenesWithHeatUI == null)
            return false;

        for (int i = 0; i < ScenesWithHeatUI.Length; i++)
        {
            if (ScenesWithHeatUI[i] == sceneName)
                return true;
        }

        return false;
    }

    void SetInteractionLocked(bool locked)
    {
        var interactor = FindFirstObjectByType<Interactor>();
        if (interactor != null)
        {
            interactor.uiLocked = locked;

            var prompt = FindFirstObjectByType<PromptUI>();
            if (prompt != null)
                prompt.Hide();
        }
    }
}