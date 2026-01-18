using UnityEngine;

public class AlertSystem : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;

    public void raiseBarkAlert(Vector3 position, Vector3 forward, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, enemyMask);
        Debug.Log("I Barked");
        for(int i = 0; i < hits.Length; i++)
        {
            HandlerAI guard = hits[i].GetComponent<HandlerAI>();
            if(guard != null)
            {
                guard.onBarkAlert(position, forward);
            }
        }
    }

    public void radioIn(Vector3 position, Vector3 forward, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, enemyMask);

        for(int i = 0; i < hits.Length; i++)
        {
            GuardAI guard = hits[i].GetComponent<GuardAI>();
            HandlerAI handler = hits[i].GetComponent<HandlerAI>();
            if(guard != null)
            {
                guard.onRadioIn(position);
            }
            if(handler != null)
            {
                handler.onRadioIn(position);
            }
        }
    }

    public void raiseRecall(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, enemyMask);

        for (int i = 0; i < hits.Length; i++)
        {
            DogAI dog1 = hits[i].GetComponent<DogAI>();
            DogAI dog2 = hits[i].GetComponent<DogAI>();
            if(dog1 != null)
            {
                dog1.onRecall(position);
            }
            if(dog2 != null)
            {
                dog2.onRecall(position);
            }
        }
    }
}
