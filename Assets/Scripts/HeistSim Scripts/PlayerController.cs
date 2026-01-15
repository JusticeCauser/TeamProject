using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [Header("----- Component -----")]
    [SerializeField] CharacterController controller; //using charactercontroller as its easier and has additional options
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 5)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod; //how much quicker you want him to go. sprinting 
    [Range(5, 20)][SerializeField] int jumpSpeed; //jump velocity- how high. Range
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 50)][SerializeField] int gravity; //bring back down

    [Header("----- Audio(SFX) -----")]
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] AudioClip[] audJump;

    int jumpCount;
    int HPOrig;

    bool isPlayingSteps;
    bool isSprinting;

    Vector3 moveDir; //vector made for movement x,y,z. wasd. instead of multiple if statements.
    Vector3 playerVel; //separately handle gravity and jump. offers more control

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if(SettingsManager.instance == null || !SettingsManager.instance.isActive)
        {
            movement();
        }

        sprint();
    }
    
    void movement()
    {
        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime); //makes it frame rate independent. one second to be one second. any time dealing with input always time delta time


        jump();
        controller.Move(playerVel * Time.deltaTime); //using jump

        playerVel.y -= gravity * Time.deltaTime; //start subtracting playerVel over time. ie pulling back down
        
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++; //counting jumps
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint")) // if pressing sprint
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint")) //if let go
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        //if(HP <= 0)
        //{
        //SettingsManager.instance.gameOver(); //not yet made
        //}
    }

    public void tazed()
    {

    }
    
    public void applyUpgrades(upgradeData upgrade)
    {
       //if we want to carry over player upgrades
    }
}
