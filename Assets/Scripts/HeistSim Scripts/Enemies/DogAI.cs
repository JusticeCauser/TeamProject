using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] HandlerAI doghandler;

    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] float alertRadius;
    [SerializeField] float barkCooldown;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float alertDur;

    //Speeds for changing animation for Dog
    [SerializeField] float roamSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float animTranSpeed;

    //Range in which dog can smell player
    bool playerInScentRange;
    bool playerInSightRange;

    float roamTimer;
    float angleToPlayer;
    float barkTimer;
    float stoppingDistOrig;
    public Transform forwardAnchor;


    //States of dog for use in transitioning the dog behavior
    public enum dogState
    {
        Idle,
        Patrol,
        Alerted,
        Chase
    }

    public dogState state = dogState.Idle;

    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 roamCenter;

    Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (difficultyManager.instance != null)
        {

            SphereCollider triggerCollider = GetComponent<SphereCollider>();
            if (triggerCollider != null && triggerCollider.isTrigger)
            {
                triggerCollider.radius *= difficultyManager.instance.GetDogDetectionMultiplier();
            }
        }

        startingPos = (doghandler != null) ? doghandler.transform.position : transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        if (GameManager.instance.player != null)
            playerTransform = GameManager.instance.player.transform;
    }

    void Update()
    {
        applyStateMovement();
        //locomotionAnim();

        switch (state)
        {
            case dogState.Idle:
                IdleBehavior();
                break;

            case dogState.Alerted:
                AlertedBehavior();
                break;

            case dogState.Chase:
                ChaseBehavior();
                break;
        }
    }

    //void locomotionAnim()
    //{
    //    float agentCurSpeed = agent.velocity.magnitude / agent.speed;
    //    float agentSpeedAnim = anim.GetFloat("Speed");
    //    anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentCurSpeed, Time.deltaTime * animTranSpeed));
    //}
    void applyStateMovement()
    {
        switch (state)
        {
            case dogState.Idle:
                agent.speed = roamSpeed;
                break;

            case dogState.Alerted:

                agent.speed = roamSpeed;
                break;

            case dogState.Chase:
                agent.speed = chaseSpeed;
                break;
        }
    }
    void IdleBehavior()
    {
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        checkRoam();

        if (playerInScentRange)
        {
            state = dogState.Alerted;
            barkTimer = 0;
            return;
        }

        if (canSeePlayer())
        {
            state = dogState.Chase;
            return;
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
        if (canSeePlayer())
        {
            return;
        }
        state = playerInScentRange ? dogState.Alerted : dogState.Idle;
    }
    bool canScentPlayer()
    {
        return playerInScentRange;
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

                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInScentRange = true;
            barkTimer = 0;

            state = dogState.Alerted;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInScentRange = false;
        }
    }
    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void bark()
    {
        //anim.SetTrigger("Bark");
        if (playerTransform == null) return;

        Vector3 pDir = playerTransform.position;
        Vector3 dir = pDir - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        GameManager.instance.alertSys.raiseBarkAlert(forwardAnchor.position, forwardAnchor.forward, alertRadius);
    }

    public void onRecall(Vector3 position)
    {
        Vector3 returnPos = position;

        agent.SetDestination(returnPos);
    }

    public void bite(GameObject playerObj)
    {
        if (Time.timeScale == 0f) return;


        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player != null && player.isHiding) return;

        GameManager.instance.missionFail(GameManager.fail.captured);
    }
    void AlertedBehavior()
    {
        if (canSeePlayer())
        {
            state = dogState.Chase;
            return;
        }

        if (playerInScentRange)
        {
            barkTimer -= Time.deltaTime;
            if (barkTimer <= 0)
            {
                bark();
                barkTimer = barkCooldown;
            }
        }
        else
        {
            state = dogState.Idle;
        }
    }
}
