using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_Guard : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    Color colorOrig;

    float shootTimer;
    float angleToPlayer;


    //Range in which guard can see player to shoot
    bool playerInSightRange;
    //Range in which guard can hear dog alert
    bool alliesInHearingRange;

    Vector3 playerDir;
    void Start()
    {
        colorOrig = model.material.color;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (playerInSightRange && canSeePlayer())
        {

        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (shootTimer >= shootRate)
                {
                    shoot();
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
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void Alert()
    {

    }
}
