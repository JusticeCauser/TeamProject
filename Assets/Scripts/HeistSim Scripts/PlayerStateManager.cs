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
    public playerState CurrentState { get; private set; } = playerState.Idle;
    
    public bool isIdle;
    public bool isSneaking;
    public bool isSprinting;
    public bool isCrouching;
    public bool isHiding;
    public bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
