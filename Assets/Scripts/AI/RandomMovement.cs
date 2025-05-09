using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : MonoBehaviour
{
    GameObject player;
    OxygenBehaviour playerOxygenBehaviour;
    Harpoon harpoon;
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
    bool playerInSight, playerInAttackRange, flee;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        playerOxygenBehaviour = player.GetComponent<OxygenBehaviour>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider>();
        harpoon = GetComponent<Harpoon>();
    }


    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSight && !playerInAttackRange)
        {
            Patrol();
        }
        else if (playerInSight && !playerInAttackRange)
        {
            Chase();
        }
        else if (playerInSight && playerInAttackRange)
        {
            Attack();
        }

        if (Vector3.Distance(transform.position, destPoint) < 5)
        {
            swimPointSet = false;
            flee = false;
        }
    }

    void Attack()
    {
        if(flee)
        {
            agent.SetDestination(destPoint);
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Moray Eel Bite Animation"))
        {
            animator.SetTrigger("Attack");
            agent.SetDestination(transform.position);
        }
    }

    void Chase()
    {
        agent.SetDestination(flee ? destPoint : player.transform.position);
    }

    void Patrol()
    {
        if (!swimPointSet)
            SearchForDest();

        if (swimPointSet)
            agent.SetDestination(destPoint);
    }

    void SearchForDest()
    {
        float z = Random.Range(-SwimRange, SwimRange);
        float x = Random.Range(-SwimRange, SwimRange);

        destPoint = new Vector3(
            transform.position.x + x,
            transform.position.y,
            transform.position.z + z);
        CreateDestPoint();
    }

    private void CreateDestPoint()
    {
        if (Physics.Raycast(destPoint, Vector3.down, out RaycastHit hit, 2000, groundLayer))
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
                    // point outside of navmesh
                    var navMeshSurface = agent.navMeshOwner.GetComponent<NavMeshSurface>();
                    var centerPos = navMeshSurface.center;
                    var maxPos = centerPos + navMeshSurface.size;
                    var minPos = centerPos - navMeshSurface.size;
                    var localDestPos = navMeshSurface.transform.InverseTransformPoint(destPoint) + centerPos;
                    localDestPos.x = Mathf.Clamp(localDestPos.x, maxPos.x, minPos.x);
                    localDestPos.y = Mathf.Clamp(localDestPos.y, maxPos.y, minPos.y);
                    localDestPos.z = Mathf.Clamp(localDestPos.z, maxPos.z, minPos.z);
                    destPoint = navMeshSurface.transform.TransformPoint(localDestPos - centerPos);
                    CreateDestPoint();
                    return;
                default:
                    break;
            }
            swimPointSet = true;
        }
    }

    public void SetDestPoint(Vector3 destPoint)
    {
        this.destPoint = destPoint;
        CreateDestPoint();
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
        var harpoon = other.gameObject.GetComponent<Harpoon>();
        Debug.Log($"Was Harpoon null: {harpoon == null}");
        if (harpoon != null)
        {
            var fleeDirection = transform.position - other.transform.position;
            fleeDirection.Normalize();
            fleeDirection *= 100;
            fleeDirection += transform.position;

            fleeDirection.y = 1000;
            SetDestPoint(fleeDirection);
            flee = true;
        }
    }

}