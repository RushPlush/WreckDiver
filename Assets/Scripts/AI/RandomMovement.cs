using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    [SerializeField] float SwimRange;

    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange, flee, swimPointSet;

    private Vector3 lastPosition;
    private List<float> distancesMoved = new();

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
        CheckIfPointIsInsideMesh(destPoint);

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
        var reach = flee ? destPoint : player.transform.position;
        Physics.Raycast(reach, Vector3.down, out var hit, 100, groundLayer);
        reach.y = agent.height / 2 + hit.point.y;
        var navMeshPath = new NavMeshPath();
        agent.CalculatePath(reach, navMeshPath);
        if (navMeshPath.status is NavMeshPathStatus.PathInvalid or NavMeshPathStatus.PathPartial)
        {
            Patrol();
            return;
        }
        agent.SetDestination(reach);
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
        if (!FindGround(out var hit)) return;
        destPoint.y = hit.point.y + agent.height / 2;
        CheckIfPointIsInsideMesh(destPoint);
    }

    private void CheckIfPointIsInsideMesh(Vector3 point)
    {
        var navMeshPath = new NavMeshPath();
        agent.CalculatePath(point, navMeshPath);
        switch (navMeshPath.status)
        {
            case NavMeshPathStatus.PathPartial:
                destPoint = navMeshPath.corners[^1];
                break;
            case NavMeshPathStatus.PathInvalid:
                // shit broke get random point
                var navMeshSurface = agent.navMeshOwner.GetComponent<NavMeshSurface>();
#if UNITY_EDITOR
                Debug.LogWarning($"Shit broke path was invalid why idk but these were the points it tried to map to: {point} correcting to {navMeshSurface.gameObject}");
#endif
                var lockedDestPoint = point - transform.position;
                var originalY = point.y;
                lockedDestPoint = Quaternion.Inverse(navMeshSurface.transform.rotation) * lockedDestPoint;
                var maxPos = navMeshSurface.center + navMeshSurface.size / 2;
                var minPos = navMeshSurface.center - navMeshSurface.size / 2;
                lockedDestPoint.x = Mathf.Clamp(lockedDestPoint.x, minPos.x, maxPos.x);
                lockedDestPoint.y = Mathf.Clamp(lockedDestPoint.y, minPos.y, maxPos.y);
                lockedDestPoint.z = Mathf.Clamp(lockedDestPoint.z, minPos.z, maxPos.z);
                point = navMeshSurface.transform.position + navMeshSurface.transform.rotation * lockedDestPoint;
                point.y = originalY;
                agent.CalculatePath(point, navMeshPath);
                switch (navMeshPath.status)
                {
                    case NavMeshPathStatus.PathInvalid:
#if UNITY_EDITOR
                        Debug.LogWarning($"Shit still broke path was invalid why idk but these were the points it tried to map to: {point}");
#endif
                        SearchForDest();
                        return;
                    case NavMeshPathStatus.PathPartial:
                        destPoint = navMeshPath.corners[^1];
                        break;
                    default:
                        destPoint = point;
                        break;
                }
                swimPointSet = true;
                return;
            default:
                break;
        }
        swimPointSet = true;
    }

    private bool FindGround(out RaycastHit hit, float distance = 500)
    {
        var ray = new Ray(transform.position, Vector3.down);
        var success = Physics.Raycast(ray, out hit, distance, groundLayer);
        if (success) return true;
        ray.direction = Vector3.up;
        return Physics.Raycast(ray, out hit, distance, groundLayer);
    }

    private void SetDestPoint(Vector3 destPoint)
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
        Debug.Log($"Was Harpoon trigger null: {harpoon == null}");
        if (harpoon && !flee)
        {
            if (harpoon.transform.IsChildOf(playerOxygenBehaviour.transform)) return;
            var fleeDirection = transform.position - playerOxygenBehaviour.transform.position;
            fleeDirection.y = 0;
            fleeDirection.Normalize();
            fleeDirection *= 100;
            fleeDirection += transform.position;

            fleeDirection.y = transform.position.y;
            SetDestPoint(fleeDirection);
            flee = true;
        }
    }

}