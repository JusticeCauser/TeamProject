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

    public void radioIn(Vector3 position)
    {
        float radius = 20f;

        Collider[] hits = Physics.OverlapSphere(position, radius, enemyMask);

        for(int i = 0; i < hits.Length; i++)
        {
            GuardAI guard = hits[i].GetComponentInParent<GuardAI>();
            HandlerAI handler = hits[i].GetComponentInParent<HandlerAI>();
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
        float radius = 20f;

        Collider[] hits = Physics.OverlapSphere(position, radius, enemyMask);

        for (int i = 0; i < hits.Length; i++)
        {
            DogAI dog = hits[i].GetComponent<DogAI>();

            if(dog != null)
            {
                dog.onRecall(position);
            }
        }
    }
}
