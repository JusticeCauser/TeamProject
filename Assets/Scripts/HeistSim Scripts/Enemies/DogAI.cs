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

    //Dog Bite Controls
    [SerializeField] float biteDist;
    [SerializeField] float biteDuration;
    

    //States of dog for use in transitioning the dog behavior
    public enum dogState
    {
        Idle,
        Patrol,
        Alerted,
        Chase,
        Recall
    }

    public dogState state = dogState.Idle;

    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 roamCenter;

    Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = (doghandler != null) ? doghandler.transform.position : transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        if (PlayerController.instance != null)
            playerTransform = PlayerController.instance.transform;
    }

    void Update()
    {
        if (GameManager.instance == null) return;
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

            case dogState.Recall:
                RecallBehavior();
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

            case dogState.Recall:
                agent.speed = roamSpeed;
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

        if (doghandler != null) startingPos = doghandler.transform.position;

        Vector3 ranPos = Random.insideUnitSphere * roamDist + startingPos;
        ranPos.y = startingPos.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(ranPos, out hit, roamDist, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
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
        if (state == dogState.Recall) return;
        Vector3 pDir = playerTransform.position;
        Vector3 dir = pDir - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
        //HeatManager.Instance.AddHeat(15f);

        GameManager.instance.alertSys.raiseBarkAlert(forwardAnchor.position, forwardAnchor.forward, alertRadius);
    }
    public void onRecall(Vector3 position)
    {
        agent.ResetPath();
        agent.SetDestination(position);
        state = dogState.Recall;
    }
    void RecallBehavior()
    {
        if(doghandler == null)
        {
            state = dogState.Idle;
            return;
        }

        Vector3 handlerpos = doghandler.transform.position;
        agent.SetDestination(handlerpos);
        float stopDist = 2f;
        if((handlerpos - transform.position).sqrMagnitude <= stopDist * stopDist)
        {
            state = dogState.Idle;
        }
    }

    public void bite(GameObject playerObj)
    {
        if (Time.timeScale == 0f) return;

        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player != null && player.isHiding) return;
        Debug.Log("I bit you!");
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
