using UnityEngine;

public class HandlerAI : EnemyAI_Base
{
    [SerializeField] guardType type;

    [SerializeField] enemyAI_Dog dog;
    [SerializeField] enemyAI_Dog dog2;

    public enum guardType
    {
        Handler,
        Elite_Handler
    }


}
