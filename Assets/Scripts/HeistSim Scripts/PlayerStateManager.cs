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
    Moving
}
public class PlayerStateManager : MonoBehaviour
{
    public playerState currentState = playerState.Idle;
    [SerializeField][Range(0,10)] float noiseLevel;
    bool isCrouching;

    bool isHiding;

    bool isMoving;
    public float speedMult { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        crouching();
        moving();
        noiseLevelChecker();

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
                else if (isCrouching != true && isMoving == true)
                {
                    currentState = playerState.Moving;
                }
                break;

            case playerState.Moving:
                if (isMoving == false)
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

                break;
        }

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
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    
    public float noiseLevelChecker()
    {
        float level = 0;
        switch(currentState)
        {
            case playerState.Idle:
                level = 0;
                break;

            case playerState.CrouchedIdle:
                level = 0;
                break;

            case playerState.Sneaking:
                level = 3;
                break;
                
            case playerState.Sprinting:
                level = 8;
                break;

            case playerState.Moving:
                level = 4;
                break;

            case playerState.Hiding:
                level = 0;
                break;
        }
        return level;
    }
}

