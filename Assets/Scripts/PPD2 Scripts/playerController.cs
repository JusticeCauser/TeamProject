//using NUnit.Framework; //causes CS0104 conflict with UnityEngine.RangeAttribute
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.DualShock;
using NUnit.Framework.Constraints;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour, IDamage, IHeal, IPickup
{
    [Header("----- Component -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    // Levi edit
    // changing HP to public for access in cheatManager.cs
    [Range(1, 10)] public int HP;
    //[Range(1, 10)] [SerializeField] int HP;
    [Range(1, 5)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 50)][SerializeField] int gravity;

   

    [Header("----- Audio(SFX) -----")]
   
    [SerializeField] AudioClip[] audSteps;

    [SerializeField] AudioClip[] audJump;
  
    bool isPlayingSteps;
    bool isSprinting;
    bool isPlayingClick;
    bool isReloading;

    private wallRun wallRun;
    bool wasWallRunning;

    int jumpCount;
    int speedOrig;
    // making HPOrig public for cheatManager.cs
    public int HPOrig;
    int gunListPos;

    // GODMODE
    public bool isGodMode = false;

    float shootTimer;
    private Coroutine chipCoroutine;

    private bool tazed;
    private float remainingTaze;

    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 externalVelocity;

    [Header("----- Grapple Settings -----")]
    [SerializeField] float groundedVelocityDecay = 15f;
    [SerializeField] float airVelocityDecay = 0.5f;

    GrapplingHook grappleHook;
    bool grappleJumpedThisFrame;

    Camera mainCam;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
       
    }
    void Start()
    {
        HPOrig = HP;
        speedOrig = speed;
        mainCam = Camera.main;
        
        grappleHook = GetComponent<GrapplingHook>();
        wallRun = GetComponent<wallRun>();

        
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!gameManager.instance.isPaused)
       // {
            movement();
       // }
        sprint();

    }

    void movement()
    {


            // only block movement when actively being pulled by grapple (not during line extend)
            bool isGrappling = grappleHook != null && grappleHook.IsGrappling();


        if (controller.isGrounded)
        {
            if(moveDir.normalized.magnitude > 0.3f && !isPlayingSteps)
            {
                StartCoroutine(playStep());
            }

            jumpCount = 0;
            if (playerVel.y <= 0)
            {
                playerVel.y = 0;
            }

            if (externalVelocity.magnitude > 0.1f)
            {
                externalVelocity = Vector3.MoveTowards(externalVelocity, Vector3.zero, groundedVelocityDecay * Time.deltaTime);
            }
            else
            {
                externalVelocity = Vector3.zero;
            }
        }
        else if (!isGrappling)
        {
            float decay = 1f - (airVelocityDecay * Time.deltaTime);
            externalVelocity *= Mathf.Max(decay, 0.9f);
        }

        if (!isGrappling)
        {
            moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

            // calculate actual velocity for wall run (moveDir * speed + external + playerVel)
            Vector3 currentVelocity = (moveDir * speed) + externalVelocity + playerVel;

            // wall run start
            if (wallRun != null)
            {
                wallRun.ProcessWallRun(ref moveDir, ref playerVel, controller.isGrounded, currentVelocity);
            }
            bool isWallRunningNow = wallRun != null && wallRun.IsWallRunning;
            if (!isWallRunningNow && wasWallRunning)
            {
                externalVelocity = Vector3.zero;
                playerVel.x = 0f;
                playerVel.z = 0f;
            }
            wasWallRunning = isWallRunningNow;
            // wall run end

            controller.Move(moveDir * speed * Time.deltaTime);

            // Wall run start
            if (wallRun == null || !wallRun.IsWallRunning)
            {
                jump();

                if (externalVelocity.magnitude > 0.1f)
                {
                    controller.Move(externalVelocity * Time.deltaTime);
                }

                controller.Move(playerVel * Time.deltaTime);
                playerVel.y -= gravity * Time.deltaTime;
            }
            else
            {
                if (externalVelocity.magnitude > 0.1f)
                {
                    controller.Move(externalVelocity * Time.deltaTime);
                }

                controller.Move(playerVel * Time.deltaTime);
            } // Wall run end
        }
        else
        {
            playerVel.y -= gravity * 0.3f * Time.deltaTime;
        }

        // Wall run start
        if (wallRun != null)
        {
            wallRun.UpdateCameraTilt();
        } // Wall run end

       
    }

    void jump()
    {
        // skip if grapple jump already happened this frame
        if (grappleJumpedThisFrame)
            return;

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && !tazed)
        {
            //aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    void LateUpdate()
    {
        grappleJumpedThisFrame = false;
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && !tazed)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && !tazed)
        {
            speed = speedOrig;
            isSprinting = false;
        }
    }

    IEnumerator playStep()
    {
        isPlayingSteps = true;
        //aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (isSprinting)
            yield return new WaitForSeconds(0.3f);
        else
            yield return new WaitForSeconds(0.5f);

        isPlayingSteps = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FrostTrap"))
        {
            damage trapSlow = other.GetComponent<damage>();
            speed = Mathf.RoundToInt(speedOrig * trapSlow.slowedSpeed);
            //gameManager.instance.frostIcon.gameObject.SetActive(true);
            //gameManager.instance.frostRing.gameObject.SetActive(true);
        }
        if (other.CompareTag("Launch Pad"))
        {
            launchPad padScript = other.GetComponent<launchPad>();
            if (padScript != null)
            {
                Vector3 launchVel = padScript.GetLaunchVelocity();
                externalVelocity = new Vector3(launchVel.x, 0f, launchVel.z);
                playerVel.y = launchVel.y;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FrostTrap"))
        {
            speed = speedOrig;
            //gameManager.instance.frostIcon.gameObject.SetActive(false);
           // gameManager.instance.frostRing.gameObject.SetActive(false);
        }
    }

    

    public void takeDamage(int amount)
    {
        // godmode
        if (isGodMode) return;

        if (amount > 0)
        {
            HP -= amount;
            //aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
            StartCoroutine(flashRed());
            updatePlayerUI();
        }

        if (HP <= 0)
        {
            // You Died!
            gameManager.instance.youLose();
        }
    }


    public void updatePlayerUI()
    { // Aaron K - added chase bar for loss of life.
        float targetFill = (float)HP / HPOrig;
        //gameManager.instance.playerHPFrontBar.fillAmount = targetFill;

        //if (chipCoroutine != null)
        //    StopCoroutine(chipCoroutine);
        //chipCoroutine = StartCoroutine(LerpBackBar(targetFill));
    }

    // Aaron K - Status Effect Icons


    IEnumerator LerpBackBar(float targetFill)
    {
        Image backBar = gameManager.instance.playerHPBackBar;
        float startFill = backBar.fillAmount;
        float duration = 0.5f;
        float time = 0;

        yield return new WaitForSeconds(0.1f);

        while (time < duration)
        {
            time += Time.deltaTime;
            backBar.fillAmount = Mathf.Lerp(startFill, targetFill, time / duration);
            yield return null;
        }
        backBar.fillAmount = targetFill;
    }

    IEnumerator flashRed()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }
    IEnumerator flashGreen() //flash green for heal
    {
        //gameManager.instance.playerHealScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f); //active flash time
        //gameManager.instance.playerHealScreen.SetActive(false);
    }

    public void heal(int healAmount)
    {
        if (HP < HPOrig)
        {
            HP = Mathf.Min(HP + healAmount, HPOrig);
            updatePlayerUI();
            StartCoroutine(flashGreen());
        }
    }

    public void SetExternalVelocity(Vector3 velocity)
    {
        externalVelocity = velocity;
    }

    public Vector3 GetVelocity()
    {
        return (moveDir * speed) + externalVelocity + playerVel;
    }

    public void SetVelocity(Vector3 velocity)
    {
        externalVelocity = new Vector3(velocity.x, 0, velocity.z);
        playerVel = new Vector3(0, velocity.y, 0);
    }

    public void GrappleJump(Vector3 grappleVelocity)
    {
        // transfer full horizontal velocity from grapple
        externalVelocity = new Vector3(grappleVelocity.x, 0, grappleVelocity.z);

        // combine upward grapple momentum with jump - keep most of it
        float upwardMomentum = Mathf.Max(grappleVelocity.y, 0);
        playerVel.y = jumpSpeed + upwardMomentum;

        // reset jump count - grapple jump gives fresh jumps (allows double jump after)
        jumpCount = 1;

        // prevent normal jump from also triggering this frame
        grappleJumpedThisFrame = true;
    }


  



    // Tazed Effect - Aaron K
    public void taze(float duration)
    {
        if (duration > remainingTaze) // to prevent short stun overriding long stun
        {
            StartCoroutine(TazeRoutine(duration));
        }
    }

    private IEnumerator TazeRoutine(float duration)
    {
        tazed = true;
        remainingTaze = duration;
        gameManager.instance.stunIcon.gameObject.SetActive(true);
        gameManager.instance.stunRing.gameObject.SetActive(true);

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            remainingTaze -= Time.deltaTime;
            speed = 0;
            gameManager.instance.stunRing.fillAmount = 1f - (timer / duration);
            yield return null;
        }

        tazed = false;
        speed = speedOrig;
        gameManager.instance.stunIcon.gameObject.SetActive(false);
        gameManager.instance.stunRing.gameObject.SetActive(false);
        remainingTaze = 0; // stop continual counting
    }

    

    // UpgradeShop stuff - JC
    public void applyUpgrade(upgradeData upgrade)
    {
        switch (upgrade.type)
        {
            // Player upgrades
            case upgradeType.playerMaxHP:
                HPOrig += Mathf.RoundToInt(upgrade.amount);
                HP = HPOrig;
                updatePlayerUI();
                break;
            case upgradeType.playerMoveSpeed:
                speedOrig += Mathf.RoundToInt(upgrade.amount);
                if (speedOrig < 1)
                    speedOrig = 1;
                speed = speedOrig;
                break;
            case upgradeType.playerJumpMax:
                jumpMax += Mathf.RoundToInt(upgrade.amount);
                if (jumpMax < 1)
                    jumpMax = 1;
                break;
          
        }
    }

    
}
