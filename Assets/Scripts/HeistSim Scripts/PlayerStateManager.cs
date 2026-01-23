using UnityEngine;
using System;


//Sneaking is just crouched on the move
public enum playerState
{
    Idle,
    Sneaking,
    CrouchedIdle,
    Sprinting,
    Hiding,
    Moving,
    Stunned
}
public class PlayerStateManager : MonoBehaviour
{
    public playerState currentState = playerState.Idle;

    [SerializeField][Range(0,10)] float noiseLevel;
    
    PlayerController player;

    bool isCrouching;
    bool isMoving;
    bool isHiding;
    bool isStunned;

    NoiseManager noiseManager;
    Crouch crouch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
        noiseManager = GetComponent<NoiseManager>();
        crouch = GetComponent<Crouch>();
    }

    // Update is called once per frame
    void Update()
    {
        hiding();
        if (isHiding)
        {
            currentState = playerState.Hiding;
            isMoving = false;
            noiseLevel = 0;
            return;
        }
        stunned();
        if(isStunned)
        {
            currentState = playerState.Stunned;
            isMoving = false;
            noiseLevel = 0;
            return;
        }
        crouching();
        moving();

        switch (currentState)
        {
            case playerState.Idle:
                if (isCrouching == true)
                {
                    currentState = playerState.CrouchedIdle;
                    if (isMoving == true)
                    {
                        currentState = playerState.Sneaking;
                    }
                }
                else if (isMoving == true)
                {
                    currentState = playerState.Moving;
                    if (Input.GetButton("Sprint") && isCrouching != true)
                    {
                        currentState = playerState.Sprinting;
                    }
                }
                break;

            case playerState.CrouchedIdle:
                if (isCrouching != true && isMoving != true)
                {
                    currentState = playerState.Idle;
                }
                else if (isCrouching != true && isMoving == true)
                {
                    currentState = playerState.Moving;
                }
                else if (isCrouching == true && isMoving == true)
                {
                    currentState = playerState.Sneaking;
                }
                    break;

            case playerState.Sneaking:

                if (isCrouching != true && isMoving != true)
                {
                    currentState = playerState.Idle;
                }
                else if (isCrouching == true && isMoving != true)
                {
                    currentState = playerState.CrouchedIdle;
                }
                else if (isCrouching != true && isMoving == true)
                {
                    currentState = playerState.Moving;
                }
                break;

            case playerState.Sprinting:
                if (isMoving != true)
                {
                    currentState = playerState.Idle;
                }
                else if (isCrouching == true && isMoving == true)
                {
                    currentState = playerState.Sneaking;
                }
                else if (!Input.GetButton("Sprint"))
                {
                    currentState = playerState.Moving;
                }
                else
                {
                    currentState = playerState.Sprinting;
                }
                break;

            case playerState.Moving:
                if (isMoving != true)
                {
                    currentState = playerState.Idle;
                }
                else if (isCrouching == true && isMoving == true)
                {
                    currentState = playerState.Sneaking;
                }
                else if (Input.GetButton("Sprint"))
                {
                    currentState = playerState.Sprinting;
                }
                break;

            case playerState.Hiding:
                if (!isHiding)
                {
                    currentState = isMoving ? playerState.Moving : playerState.Idle;
                }
                    break;
            case playerState.Stunned:
                if (!isStunned)
                {
                    currentState = isMoving ? playerState.Moving : playerState.Idle;
                }
                break;
        }

        noiseLevelChecker();

    }

    void crouching()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
        }
    }

    void moving()
    {
        if (isHiding == true) return;

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    void hiding()
    {
        if (player.isHiding == true)
        {
            isHiding = true;
        }
        else
        {
            isHiding = false;
        }
    }
    
    void stunned()
    {
        if(player.isStunned == true)
        {
            isStunned = true;
        }
        else
        {
            isStunned = false;
        }
    }
    public float noiseLevelChecker()
    {
        
        //switch(currentState)
        //{
        //    case playerState.Idle:
        //        noiseLevel = 0;
        //        break;
        //
        //    case playerState.CrouchedIdle:
        //        noiseLevel = 0;
        //        break;
        //
        //    case playerState.Sneaking:
        //        noiseLevel = 3;
        //        break;
        //        
        //    case playerState.Sprinting:
        //        noiseLevel = 8;
        //       break;
        //
        //    case playerState.Moving:
        //        noiseLevel = 4;
        //        break;
        //
        //    case playerState.Hiding:
        //        noiseLevel = 0;
        //        break;
        //
        //    case playerState.Stunned:
        //        noiseLevel = 0;
        //        break;
        //}

        if (noiseManager == null)
        {
            noiseLevel = 0;
            return noiseLevel;
        }

        noiseLevel = Mathf.Clamp(noiseManager.noise / 10f, 0f, 10f);
        return noiseLevel;
    }
}

