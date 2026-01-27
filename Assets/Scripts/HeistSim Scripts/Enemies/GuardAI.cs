using UnityEngine;

public class GuardAI : EnemyAI_Base
{
    [SerializeField] GuardAI guard1;
    [SerializeField] GuardAI guard2;
    [SerializeField] guardType type;
    [SerializeField] float stunDuration = 1f;
    [SerializeField] float tazeCooldown = 5f;
    [SerializeField] float tazeDist;

    float nextTazeTime;

    public enum guardType
    {
        Standard,
        Elite
    }

    protected override void tryToTaze()
    {
        toTaze();
    }

    public void toTaze()
    {
        if (playerTransform == null) return;

        float distance = (playerTransform.position - transform.position).sqrMagnitude;

        if (state != guardState.Chase) return;

        if (distance > tazeDist * tazeDist) return;

        PlayerController player = playerTransform.GetComponent<PlayerController>();

        if (player == null) return;
        if (player.isStunned) return;
        if (Time.time < nextTazeTime) return;
        nextTazeTime = Time.time + tazeCooldown;

        player.tazed(stunDuration);
    }
}