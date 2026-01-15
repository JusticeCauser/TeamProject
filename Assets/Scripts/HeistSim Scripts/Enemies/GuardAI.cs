using UnityEngine;

public class GuardAI : EnemyAI_Base
{
    [SerializeField] GuardAI guard1;
    [SerializeField] GuardAI guard2;
    [SerializeField] guardType type;

    public enum guardType
    {
        Standard,
        Elite
    }

    public void onAllyAlert(Vector3 alertPosition)
    {
        agent.SetDestination(alertPosition);
    }

    public void onAlert()
    {

        if (guard1 != null)
        {
            guard1.onAllyAlert(playerTransform.position);
        }
        if (guard2 != null)
        {
            guard2.onAllyAlert(playerTransform.position);
        }
    }
}
