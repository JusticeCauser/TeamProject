using UnityEngine;
using UnityEngine.UIElements;
using static enemyAI_Guard_Handler;

public class HandlerAI : EnemyAI_Base
{
    [SerializeField] guardType type;

    [SerializeField] DogAI dog;
    [SerializeField] DogAI dog2;

    Vector3 currentPos;

    public enum guardType
    {
        Handler,
        Elite_Handler
    }

    public void onBarkAlert(Vector3 position, Vector3 anchor)
    {
        if (HeatManager.Instance != null)
        {
            HeatManager.Instance.AddHeat(5f);
        }

        alertTargetPos = position;
        Debug.Log("I heard Bark");
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

    public void recall()
    {
        Vector3 handlerPos = transform.position;

        if (dog != null)
        {
            Vector3 d = dog.transform.position - handlerPos;
            if (d.sqrMagnitude > 15f)
                dog.onRecall(handlerPos);
        }

        if (dog2 != null)
        {
            Vector3 d2 = dog2.transform.position - handlerPos;
            if (d2.sqrMagnitude > 15f)
                dog2.onRecall(handlerPos);
        }
    }
}
