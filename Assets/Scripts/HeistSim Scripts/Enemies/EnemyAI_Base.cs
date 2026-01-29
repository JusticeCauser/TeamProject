using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;


public class EnemyAI_Base : MonoBehaviour
{
    //This allows the Enemies to interact with the playerStateManager
    protected PlayerStateManager playerStateManager;
    //Here we have our basics: Model, ability to move, and the ability to dance and preform other movements
    [SerializeField] Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask hidingSpotMask;
    IHide targetHide;
    Transform currentHidePos;
    GuardAI guard;
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
    [SerializeField] float sweepSpeed;
    [SerializeField] float hideSearchRadius;

    [Header("Drag")]
    public bool isBeingDragged;
    [SerializeField] Transform dragAttachPoint;

    float hearingRadius = 15;
    float hearingInterval = 0.2f;
    float nextHearingTime;
    float suspicion;
    float suspicionDecay = 2f;
    float suspiciousTimer;
    float suspiciousToSearch = 5f;
    float suspicionToSuspicious = 1.5f;
    float searchTimer;
    float lastHeardTimer;
    float minPerceivedNoise = 1f;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    public float alertedTimer;
    float lookTimer;
    protected int destPatrolPoints;
    float canHearTimer;
    protected int destHidingSpots;
    bool heardAgain;
    public bool radioIn;
    public float radioInTimer;
    public float radioInDuration = 2f;
    public enum guardState
    {
        Idle,
        Patrol,
        Search,
        Suspicious,
        Alerted,
        Chase,
        KnockedOut,
        Hunt
    }

    public guardState state = guardState.Idle;
    public guardState previousState;

    bool playerInSightRange;
    [HideInInspector] public bool playerInHearingRange;
    public bool isKnockedOut => state == guardState.KnockedOut;

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
        if (PlayerController.instance != null)
            playerTransform = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance == null) return;

        if (playerTransform == null)
        {
            if (PlayerController.instance != null)
                playerTransform = PlayerController.instance.transform;
            else
                return;
        }

        if (playerStateManager == null)
        {
            playerStateManager = playerTransform.GetComponent<PlayerStateManager>();
            if (playerStateManager == null) return;
        }
        // locomotionAnim();

        if (isKnockedOut || isBeingDragged) return;

        applyStateMovement();

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
            case guardState.Hunt:
                HuntBehavior();
                break;
            case guardState.KnockedOut:
                break;
        }

        if (playerInHearingRange == true && state != guardState.Chase && state != guardState.Hunt)
        {
            sampleHearing();
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

    public void takeKnockOut(float duration)
    {
        if (state == guardState.KnockedOut) return;
        //crandom comment 1
        StopAllCoroutines();
        StartCoroutine(knockedOut(duration));
    }

    IEnumerator knockedOut(float duration)
    {
        previousState = state;
        state = guardState.KnockedOut;

        agent.isStopped = true;
        agent.ResetPath();

        yield return new WaitForSeconds(duration);

        agent.isStopped = false;

        state = (patrolPoints != null && patrolPoints.Length > 0) ? guardState.Patrol : guardState.Idle;
    }
    void nextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[destPatrolPoints].position);
        destPatrolPoints = (destPatrolPoints + 1) % patrolPoints.Length;
    }

    void SearchBehavior()
    {
        if (canSeePlayer())
        {
            lastAlertPosition = playerTransform.position;
            if (!radioIn && GameManager.instance != null)
            {
                radioInTimer += Time.deltaTime;
                onRadioIn(lastAlertPosition);
                radioIn = true;
                if(radioInTimer >= radioInDuration)
                {
                    radioIn = false;
                }
                
            }
            state = guardState.Chase;
            return;
        }
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchDur)
        {
            searchTimer = 0;
            radioIn = false;
            state = guardState.Patrol;
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            //Transform hide = findHidingSpot();
            //if (hide != null)
            //{
            //    currentHidePos = hide;
            //    targetHide = hide.GetComponent<IHide>();
            //    agent.SetDestination(hide.position);
            //    if (targetHide != null && targetHide.hasPlayer)
            //    {
            //        hideCapture(targetHide.occupied.gameObject);
            //    }
            //}
            //else
            //{
                roam(lastAlertPosition, roamDist);
            //}
        }
    }

    void HuntBehavior()
    {
        roamDist = 20;
        lastAlertPosition = playerTransform.position;
        agent.SetDestination(lastAlertPosition);

        if (canSeePlayer())
        {
            lastAlertPosition = playerTransform.position;
            if (!radioIn && GameManager.instance != null)
            {
                radioInTimer += Time.deltaTime;
                onRadioIn(lastAlertPosition);
                radioIn = true;
                if (radioInTimer >= radioInDuration)
                {
                    radioIn = false;
                }
            }
            state = guardState.Chase;
            return;
        }
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchDur)
        {
            searchTimer = 0;
            radioIn = false;
            state = guardState.Patrol;
            return;
        }

        float huntTimer = 0;
        float huntDur = 10f;
        huntTimer += Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            //Transform hide = findHidingSpot();
            //if (hide != null)
            //{
            //    agent.SetDestination(hide.position);
            //}
            //else
            //{
                roam(lastAlertPosition, roamDist);
            //}
        }
    }
    //Transform findHidingSpot()
    //{
    //    Collider[] hits = Physics.OverlapSphere(transform.position, hideSearchRadius, hidingSpotMask);

    //    float bDist = Mathf.Infinity;
    //    Transform b = null;

    //    foreach (var i in hits)
    //    {
    //        float dist = (i.transform.position - transform.position).sqrMagnitude;
    //        if (dist < bDist)
    //        {
    //            bDist = dist;
    //            b = i.transform;
    //        }
    //    }
    //    return b;
    //}
    void SuspiciousBehavior()
    {
        if (canSeePlayer())
        {
            agent.isStopped = false;
            state = guardState.Chase;
            suspiciousTimer = 0f;
            return;
            if (radioInTimer >= radioInDuration)
            {
                radioIn = false;
            }
        }

        lastAlertPosition = playerTransform.position;
        if (!radioIn && GameManager.instance != null)
        {
            radioInTimer += Time.deltaTime;
            GameManager.instance.alertSys.radioIn(lastAlertPosition);
            radioIn = true;
            if (radioInTimer >= radioInDuration)
            {
                radioIn = false;
            }
        }

        agent.isStopped = true;
        suspiciousTimer += Time.deltaTime;

        if (state != guardState.Suspicious)
        {
            agent.isStopped = false;
            suspiciousTimer = 0;
            return;
        }

        if (suspiciousTimer >= suspiciousMax)
        {
            agent.isStopped = false;
            suspiciousTimer = 0f;
            suspicion = 0;
            state = previousState;
            return;
        }
    }
    void PatrolBehavior()
    {
        if (canSeePlayer())
        {
            state = guardState.Chase;
        }
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            nextPoint();
        }
    }
    void IdleBehavior()
    {
        if (canSeePlayer())
        {
            state = guardState.Chase;
        }
    }

    //void checkRoam()
    //{
    //    if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
    //    {
    //        roam();
    //    }
    //}
    void roam(Vector3 center, float radius)
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(ranPos, out hit, roamDist, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void ChaseBehavior()
    {
        agent.isStopped = false;
        lastAlertPosition = playerTransform.transform.position;
        if (!radioIn && GameManager.instance != null)
        {
            radioInTimer += Time.deltaTime;
            GameManager.instance.alertSys.radioIn(lastAlertPosition);
            radioIn = true;
            if (radioInTimer >= radioInDuration)
            {
                radioIn = false;
            }
        }

        if (!canSeePlayer())
        {
            state = guardState.Alerted;
        }
        
            tryToTaze();
    }

    protected virtual void tryToTaze()
    {


    }
    bool canSeePlayer()
    {
        if (playerTransform == null) return false;
        var pc = PlayerController.instance;
        if (pc != null && pc.isHiding) return false;

        Vector3 playerPos = playerTransform.position;
        playerDir = playerPos - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                if (!radioIn && GameManager.instance != null)
                {
                    radioInTimer += Time.deltaTime;
                    GameManager.instance.alertSys.radioIn(playerPos);
                    radioIn = true;
                    if (radioInTimer >= radioInDuration)
                    {
                        radioIn = false;
                    }
                }
                agent.SetDestination(playerPos);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
                }
                //agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        return false;
    }

    public void sampleHearing()
    {
        if (state == guardState.KnockedOut) return;
        if (isBeingDragged == true) return;

        if (playerTransform == null) return;
        if (playerInHearingRange == false) return;

        if (state == guardState.Chase || state == guardState.Hunt) return;

        if (Time.time < nextHearingTime) return;
        nextHearingTime = Time.time + hearingInterval;

        float noiseLevel = playerStateManager.noiseLevelChecker();
        if (!radioIn && GameManager.instance != null)
        {
            radioInTimer += Time.deltaTime;
            GameManager.instance.alertSys.radioIn(lastAlertPosition);
            radioIn = true;
            if (radioInTimer >= radioInDuration)
            {
                radioIn = false;
            }
        }

        if (noiseLevel <= 0)
        {
            suspicion = Mathf.Max(0, suspicion - suspicionDecay * hearingInterval);
            return;
        }

        float distance = Vector3.Distance(agent.transform.position, playerTransform.position);

        float t = Mathf.Clamp01(distance / hearingRadius);

        float falloff = (1 - t) * (1 - t);
        float perceivedNoise = noiseLevel * falloff;

        if (perceivedNoise < minPerceivedNoise)
        {
            suspicion = Mathf.Max(0, suspicion - suspicionDecay * hearingInterval);
            return;
        }
        suspicion = suspicion + (perceivedNoise * hearingInterval);
        suspicion = Mathf.Clamp(suspicion, 0, suspiciousMax);

        lastHeardPosition = playerTransform.position;
        lastHeardTimer = Time.time;

        if (state == guardState.Idle || state == guardState.Patrol || state == guardState.Search)
        {
            previousState = state;
            state = guardState.Suspicious;
            suspiciousTimer = 0;
            return;
        }

        if (state == guardState.Suspicious)
        {
            state = guardState.Alerted;
            alertedTimer = 0;
            return;
        }
        if (state == guardState.Alerted)
        {
            return;
        }
        if (canSeePlayer())
        {
            state = guardState.Chase;
            return;
        }
        return;
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
        agent.isStopped = true;


        lastHeardPosition = playerTransform.position;
        playerDir = lastHeardPosition - transform.position;
        playerDir.y = 0f;
        if (playerDir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(playerDir);
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
        }

        alertedTimer = alertedTimer + Time.deltaTime;

        if (canSeePlayer())
        {
            state = guardState.Chase;
            return;
        }

        if (alertedTimer >= alertDur)
        {
            agent.isStopped = false;
            alertedTimer = 0;
            state = guardState.Search;
            return;
        }
    }

    public void Capture(GameObject playerObj)
    {
        if (Time.timeScale == 0f) return;

        if (state == guardState.KnockedOut) return;
        if (isBeingDragged == true) return;

        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player != null && player.isHiding) return;

        GameManager.instance.missionFail(GameManager.fail.captured);
    }

    public void hideCapture(GameObject playerObj)
    {
        if (Time.timeScale == 0f) return;

        if (state == guardState.KnockedOut) return;
        if (isBeingDragged == true) return;

        PlayerController player = playerObj.GetComponent<PlayerController>();

        GameManager.instance.missionFail(GameManager.fail.captured);
    }
    public void onRadioIn(Vector3 position)
    {
        alertTargetPos = position;

        agent.SetDestination(alertTargetPos);
        state = guardState.Search;
    }
}