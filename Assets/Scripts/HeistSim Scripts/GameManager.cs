using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    float timeScaleOrig;
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

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void missionComplete()
    {
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
