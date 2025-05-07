using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; 

public class RandomMovement : MonoBehaviour 
{
    GameObject player;
    OxygenBehaviour playerOxygenBehaviour;
    NavMeshAgent agent;
    Animator animator;
    BoxCollider boxCollider;

    [SerializeField] LayerMask groundLayer, playerLayer;

    //Swimming patrolling
    Vector3 destPoint;
    bool swimPointSet;
    [SerializeField] float SwimRange;

    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        playerOxygenBehaviour= player.GetComponent<OxygenBehaviour>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider>();

    }


    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSight && !playerInAttackRange)
        {
            Patrol();
        }
        if (playerInSight && !playerInAttackRange)
        {
            Chase();
        }
        if (playerInSight && playerInAttackRange)
        {
            Attack();
        }  
    }

    void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Moray Eel Bite Animation"))
        {
            animator.SetTrigger("Attack");
            agent.SetDestination(transform.position);
        }
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Patrol()
    {
        if (!swimPointSet)
            SearchForDest();
        
        if (swimPointSet) 
            agent.SetDestination(destPoint);

        if (Vector3.Distance(transform.position, destPoint)  < 5) 
            swimPointSet = false;
    }

    void SearchForDest ()
    {
        float z = Random.Range(-SwimRange, SwimRange);
        float x = Random.Range(-SwimRange, SwimRange);

        destPoint = new Vector3(
            transform.position.x + x, 
            transform.position.y, 
            transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, out RaycastHit hit, groundLayer))
        {
            destPoint.y = hit.point.y + agent.height / 2;
            var navMeshPath = new NavMeshPath();
            agent.CalculatePath(destPoint, navMeshPath);
            switch (navMeshPath.status)
            {
                case NavMeshPathStatus.PathPartial:
                    destPoint = navMeshPath.corners[^1];
                    break;
                case NavMeshPathStatus.PathInvalid:
                    return;
                default:
                    break;
            }
            swimPointSet = true;
        }
    }

    void EnableAttack()
    {
        boxCollider.enabled = true;
    }

    void DisableAttack()
    {
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Movement>();

        double biteDamage = 30;

        if (player != null)
        {
            print("Hit!");
            playerOxygenBehaviour.LoseOxygen(biteDamage);
        }
    }
}