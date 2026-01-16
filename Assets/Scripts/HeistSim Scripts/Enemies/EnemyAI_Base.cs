using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static enemyAI_Guard;

public class EnemyAI_Base : MonoBehaviour
{
    //This allows the Enemies to interact with the playerStateManager
    protected PlayerStateManager playerStateManager;
    //Here we have our basics: Model, ability to move, and the ability to dance and preform other movements
    [SerializeField] Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] Animator anim;
    //These fields allow the enemy to have speed, sight, and other important things - like time spent alert or searching
    [SerializeField] int animTranSpeed;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float alertDur;
    [SerializeField] float searchDur;
    [SerializeField] float roamSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float baseHearing;

    float searchTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    float alertedTimer;

    public enum guardState
    {
        Idle,
        Patrol,
        Search,
        Alerted,
        Chase
    }

    public guardState state = guardState.Idle;

    bool playerInSightRange;
    [HideInInspector] public bool playerInHearingRange;

    Vector3 playerDir;
    Vector3 alertTargetPos;
    Vector3 alertLookDir;
    Vector3 lastAlertPosition;
    Vector3 lastHeardPosition;
    Vector3 startingPos;

    protected Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

        if (gameManager.instance.player != null)
            playerTransform = gameManager.instance.player.transform;

        playerStateManager = playerTransform.GetComponent<PlayerStateManager>();
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
    public bool canHearPlayer()
    {
        if(playerTransform == null) return false;

        Vector3 playerPos = playerTransform.position;
        playerDir = playerPos - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        return true;
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
