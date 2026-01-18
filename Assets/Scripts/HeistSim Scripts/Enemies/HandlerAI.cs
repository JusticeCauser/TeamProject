using UnityEngine;
using UnityEngine.UIElements;
using static enemyAI_Guard_Handler;

public class HandlerAI : EnemyAI_Base
{
    [SerializeField] guardType type;

    [SerializeField] DogAI dog;
    [SerializeField] DogAI dog2;

    public enum guardType
    {
        Handler,
        Elite_Handler
    }

    public void onBarkAlert(Vector3 position, Vector3 anchor)
    {
        alertTargetPos = position;

        Vector3 playerDir = anchor;
        playerDir.y = 0;

        if (playerDir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(playerDir);
            transform.rotation = rot;
        }
        //moves guard toward anchor
        agent.stoppingDistance = 0;
        agent.SetDestination(alertTargetPos);
        //sets state to searching
        state = guardState.Search;
        alertedTimer = 0;
    }

    public void onRadioIn(Vector3 position)
    {
        alertTargetPos = position;

        agent.SetDestination(alertTargetPos);
        state = guardState.Search;
    }
}
