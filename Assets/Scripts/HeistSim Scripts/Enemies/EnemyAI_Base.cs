using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;
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
    public Transform[] patrolPoints;
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
    [SerializeField] float suspiciousMax;

    float hearingSphreRadius;
    float searchTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    public float alertedTimer;
    float suspiciousTimer;
    float lookTimer;
    protected int destPatrolPoints;

    public enum guardState
    {
        Idle,
        Patrol,
        Search,
        Suspicious,
        Alerted,
        Chase
    }

    public guardState state = guardState.Idle;
    public guardState previousState;

    bool playerInSightRange;
    [HideInInspector] public bool playerInHearingRange;

    protected Vector3 playerDir;
    protected Vector3 alertTargetPos;
    protected Vector3 alertLookDir;
    protected Vector3 lastAlertPosition;
    protected Vector3 lastHeardPosition;
    protected Vector3 startingPos;
    protected Vector3 pointApos;
    protected Vector3 pointBpos;

    protected Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

        if (GameManager.instance.player != null)
            playerTransform = GameManager.instance.player.transform;

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
            case guardState.Search:
                SearchBehavior();
                break;
            case guardState.Suspicious:
                SuspiciousBehavior();
                break;
            case guardState.Patrol:
                PatrolBehavior();
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
    void nextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[destPatrolPoints].position);
        destPatrolPoints = (destPatrolPoints + 1) % patrolPoints.Length;
    }
    void SearchBehavior()
    {

    }
    void SuspiciousBehavior()
    {
        suspiciousTimer += Time.deltaTime;

        lastHeardPosition = playerTransform.position;
        playerDir = lastHeardPosition - transform.position;
        playerDir.y = 0f;
        
        if(suspiciousTimer < suspiciousMax)
        {
            agent.isStopped = true;
            float newNoise = playerStateManager.noiseLevelChecker();

            if(newNoise > 2f)
            { 
                agent.isStopped = false;
                state = guardState.Alerted;
                return;
            }
        }
        else if (suspiciousTimer >= suspiciousMax)
        {
            agent.isStopped = false;
            suspiciousTimer = 0f;
            state = previousState;
        }
    }
    void PatrolBehavior()
    {
        destPatrolPoints = 0;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            {
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[destPatrolPoints].position);
                destPatrolPoints = (destPatrolPoints + 1) % patrolPoints.Length;
                state = guardState.Patrol;
            }
        }
        if (canSeePlayer())
        {
            state = guardState.Chase;
        }
        if (agent.pathPending) return;

        if(agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            nextPoint();
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
        if (playerTransform == null) return false;

        float noiseLevel = playerStateManager.noiseLevelChecker();

        bool seePlayer = canSeePlayer();
        if(noiseLevel <= 0)
        {
            return false;
        }
        if(noiseLevel >= 2 && noiseLevel < 5)
        {
            if (seePlayer)
            {
                state = guardState.Chase;
                return true;
            }
            if (state != guardState.Suspicious)
            {
                suspiciousTimer = 0f;
                previousState = state;
                state = guardState.Suspicious;
            }
        }
        if(noiseLevel >= 5 && noiseLevel <= 10)
        {

        }
        lastHeardPosition = playerTransform.position;
        playerDir = lastHeardPosition - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        return true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("EnteringTrigger");
            playerInSightRange = true;
            //playerInHearingRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSightRange = false;
            playerInHearingRange = false;
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
