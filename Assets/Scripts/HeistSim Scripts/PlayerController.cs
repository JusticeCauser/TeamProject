//using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections;


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

    [Header("---Items---")]
    [SerializeField] int maxInventory = 7;
    [SerializeField] List<itemStats> itemList = new List<itemStats>();
    //[SerializeField] Transform droppedItem; for dropping items

    [Header("---Enemy---")]
    [SerializeField] Transform dragPoint;

    EnemyAI_Base draggedEnemy;
    public bool isDragging => draggedEnemy != null;

    int jumpCount;
    int HPOrig;
    int itemListPos;

    public int totalValue;
    public bool isStunned;
    bool isPlayingSteps;
    bool isSprinting;
    public bool isHiding;

    Vector3 moveDir; //vector made for movement x,y,z. wasd. instead of multiple if statements.
    Vector3 playerVel; //separately handle gravity and jump. offers more control
    Vector3 externalVelocity;

    [SerializeField] float crouchSpeed = 2f;
    Crouch crouch;
    KeypadUI keypad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        instance = this;
        crouch = GetComponent<Crouch>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isStunned || isHiding || SettingsManager.instance == null || !SettingsManager.instance.isActive)
        {
            movement();
        }

        sprint();
        selectItem();
        draggingEnemy();
    }

    void movement()
    {
        if (isHiding == true) return;
        if (isStunned == true) return;

        float actualSpeed = speed; // needed for conversion

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        if (crouch != null && crouch.IsCrouching)
            actualSpeed = crouchSpeed;

        controller.Move(moveDir * actualSpeed * Time.deltaTime); //makes it frame rate independent. one second to be one second. any time dealing with input always time delta time

        jump();
        controller.Move(playerVel * Time.deltaTime); //using jump

        playerVel.y -= gravity * Time.deltaTime; //start subtracting playerVel over time. ie pulling back down

    }

    void jump()
    {
        if (keypad != null && keypad.IsOpen)
            return;

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

    public Vector3 GetVelocity()
    {
        return (moveDir * speed) + externalVelocity + playerVel;
    }
    public void SetExternalVelocity(Vector3 velocity)
    {
        externalVelocity = velocity;
        //externalVelocity = new Vector3(velocity.x, 0, velocity.z);
    }
    public void GrappleJump(Vector3 grappleVelocity)
    {
        playerVel = grappleVelocity;
        playerVel.y += jumpSpeed * .7f;
        jumpCount = 0;
    }
    //public void takeDamage(int amount)
    //{
    //    HP -= amount;

    //    if (HP <= 0)
    //    {

    //        //SettingsManager.instance.gameOver(); //not yet made
    //    }
    //}
    public void tazed(float duration)
    {
        if(isStunned == true) return;

        StartCoroutine(isTazed(duration));
    }

    IEnumerator isTazed(float duration)
    {
        isStunned = true;

        yield return new WaitForSecondsRealtime(duration);

        isStunned = false;
    }
    public void hide()
    {
        isHiding = true;
        controller.enabled = false;
    }
    public void exitHide()
    {
        isHiding = false;
        controller.enabled = true;
    }
    public void grabItem(itemStats item)
    {
        if (itemList.Count >= maxInventory)
            return;

        itemList.Add(item);
        itemListPos = itemList.Count - 1;

        totalValue += item.itemValue;
    }
  
    void selectItem()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && itemListPos < itemList.Count - 1)
        {
            itemListPos++;
        }
        if(Input.GetAxis("Mouse ScrollWheel") <  0 && itemListPos > 0)
        {
            itemListPos--;
        }
    }
    public void applyUpgrades(upgradeData upgrade)
    {
        //if we want to carry over player upgrades
    }

    void draggingEnemy()
    {
        if (draggedEnemy != null)
        {
            draggedEnemy.transform.position = Vector3.Lerp(
                draggedEnemy.transform.position,
                dragPoint.position,
                Time.deltaTime * 12f
            );

            draggedEnemy.transform.rotation = Quaternion.Lerp(
                draggedEnemy.transform.rotation,
                dragPoint.rotation,
                Time.deltaTime * 12f
            );
        }
    }

    public void startDrag(EnemyAI_Base enemy)
    {
        draggedEnemy = enemy;
        enemy.isBeingDragged = true;
    }
    public void stopDrag()
    {
        if (draggedEnemy != null)
            draggedEnemy.isBeingDragged = false;

        draggedEnemy = null;
    }
}