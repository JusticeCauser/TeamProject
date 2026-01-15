using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using static enemyAI_Guard;

public class EnemyAI_Base : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] int animTranSpeed;
    [SerializeField] int HP;
    [SerializeField] int maxHP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float alertDur;

    [SerializeField] float roamSpeed;
    [SerializeField] float chaseSpeed;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    float alertedTimer;

    public enum guardState
    {
        Idle,
        Patrol,
        Alerted,
        Chase
    }

    public guardState state = guardState.Idle;

    bool playerInSightRange;

    Vector3 playerDir;
    Vector3 alertTargetPos;
    Vector3 alertLookDir;
    Vector3 lastAlertPosition;
    Vector3 startingPos;

    protected Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHP = HP;

        if (difficultyManager.instance != null)
        {
            HP = Mathf.RoundToInt(HP * difficultyManager.instance.GetHealthMultiplier());
        }

        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

        if (gameManager.instance.player != null)
            playerTransform = gameManager.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        applyStateMovement();
       // locomotionAnim();

        switch (state)
        {
            case guardState.Idle:
                IdleBehavior();
                break;

            case guardState.Alerted:
                AlertedBehavior();
                break;

            case guardState.Chase:
                ChaseBehavior();
                break;
        }
    }
    void locomotionAnim()
    {
        float agentSpeedCur = agent.velocity.magnitude / agent.speed;
        float agentSpeedAnim = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentSpeedCur, Time.deltaTime * animTranSpeed));
    }
    void applyStateMovement()
    {
        switch (state)
        {
            case guardState.Idle:
                agent.speed = roamSpeed;
                break;

            case guardState.Alerted:

                agent.speed = roamSpeed;
                break;

            case guardState.Chase:
                agent.speed = chaseSpeed;
                break;
        }
    }
    void IdleBehavior()
    {
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInSightRange && canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInSightRange)
        {
            checkRoam();
        }
        else
        {
            state = guardState.Chase;
        }
    }

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            roam();
        }
    }
    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    void ChaseBehavior()
    {
        if (!canSeePlayer())
        {
            state = guardState.Alerted;
        }
    }
    bool canSeePlayer()
    {
        if (playerTransform == null) return false;

        Vector3 playerPos = playerTransform.position;
        playerDir = playerPos - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(playerPos);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
                }
                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSightRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSightRange = false;
        }
    }
    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void AlertedBehavior()
    {
        if (canSeePlayer())
        {
            state = guardState.Chase;
            return;
        }
        if (agent.remainingDistance <= 0.1f)
        {
            alertedTimer += Time.deltaTime;

            if (alertedTimer >= alertDur)
            {
                state = guardState.Idle;
            }
        }
    }
}
