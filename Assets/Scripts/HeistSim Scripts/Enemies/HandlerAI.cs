using UnityEngine;
using UnityEngine.UIElements;
using static enemyAI_Guard_Handler;

public class HandlerAI : EnemyAI_Base
{
    [SerializeField] guardType type;

    [SerializeField] enemyAI_Dog dog;
    [SerializeField] enemyAI_Dog dog2;

    Vector3 alertTargetPos;
    Vector3 alertLookDir;
    public enum guardType
    {
        Handler,
        Elite_Handler
    }

    void onBarkAlert(Vector3 position, Vector3 anchor)
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
        //sets state to alerted
        state = guardState.Alerted;
        alertedTimer = 0;
    }
}
