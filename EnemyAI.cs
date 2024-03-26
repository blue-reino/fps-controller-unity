
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    Rigidbody rb;
    public float hitForceThreshold = 20.0f; 
    public Transform player;
    public LayerMask Ground, whatIsPlayer;

    //Enemy Stats
    public int enemyHealth = 100;


    //Audio
    public AudioSource zSound1Agro;
    public AudioSource zSound2Chase;



    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public GameObject projectile;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;


    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake(){
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
         rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    private void Update(){
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

    }

    private void Patrolling(){
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            if (agent.enabled == true) agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;   

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
{
    // Calculate random point within range
    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    float randomX = Random.Range(-walkPointRange, walkPointRange);

    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    RaycastHit hit;
    if (Physics.Raycast(walkPoint, -transform.up, out hit, 2f, Ground))
    {
        walkPointSet = true;
    }
}

    private bool isAudioPlaying = false; // Flag to track if audio is currently playing

    private void ChasePlayer()
    {
        if (agent.enabled == true)
        {
            // Check if the audio is not already playing and not in cooldown period
            if (!isAudioPlaying && !IsAudioCoolingDown)
            {
                zSound1Agro.enabled = true;
                zSound1Agro.Play();
                StartCoroutine(AudioCooldown(Random.Range(10f, 20f))); // Start the cooldown
            }

            // Set the destination to chase the player
            agent.SetDestination(player.position);
        }
    }

    private IEnumerator AudioCooldown(float cooldownDuration)
    {
        isAudioPlaying = true; // Set flag to indicate audio is playing

        yield return new WaitForSeconds(cooldownDuration);

        // Reset flags after cooldown
        isAudioPlaying = false;
    }

    private bool IsAudioCoolingDown
    {
        get { return isAudioPlaying; } // Check if audio is currently playing
    }

    private void AttackPlayer(){
        //Set enemy movment to false
        if (agent.enabled == true) agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked) {

            //Attack Code Here
            /**
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            **/


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("knockDown"))
        {
            /**
            // Stop the NavMeshAgent
            agent.isStopped = true;
            // Optionally, you can disable the NavMeshAgent component
            agent.enabled = false;
            // Apply a force to the Rigidbody, if it exists
            if (rb != null)
            {
                // Calculate the force direction (away from the collision point)
                Vector3 forceDirection = collision.contacts[0].point - transform.position;
                forceDirection.Normalize();
                float hitForceMagnitude = 15.0f;          
                if (rb.velocity.magnitude < hitForceThreshold)
                {
                    hitForceMagnitude = hitForceThreshold - rb.velocity.magnitude;
                }
                // Apply the force with the hit force threshold
                rb.AddForce(forceDirection * hitForceMagnitude, ForceMode.Impulse);

            }
            StartCoroutine(EnableNavMeshAgentAfterDelay(2f));
            **/
            enemyHealth -= 50;
            if (enemyHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    /**
    private IEnumerator EnableNavMeshAgentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Re-enable the NavMeshAgent
        agent.isStopped = false;

        agent.enabled = true;
    }
    **/



}
