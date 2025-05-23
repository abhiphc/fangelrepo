using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float distanceToPlayer;
    [SerializeField] float chaseDistance = 20f; // Distance to start chasing the player
    [SerializeField] float attackDistance = 2f; // Distance to attack the player
    [SerializeField] List<Transform> patrolPoints = new List<Transform>();
    GameObject player;
    [SerializeField] bool isPatrolling;
    [SerializeField] bool canChaseAndAttack;
    private int patrolIndex = 0;
    Animator animator;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        isPatrolling = true;
        canChaseAndAttack = false;
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        StartPatrol();
        ChasingAndAttacking();
        
    }
    void StartPatrol()
    {
        if (isPatrolling)
        {
            canChaseAndAttack = false; // Stop chasing when patrolling
            animator.SetBool("isAttacking", false);
            agent.SetDestination(patrolPoints[patrolIndex].position);
            if(Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < 1f)
            {
                patrolIndex++;
                if (patrolIndex >= patrolPoints.Count)
                {
                    patrolIndex = 0; // Loop back to the first point
                }
            }
           
        }
    }
    void ChasingAndAttacking()
    {
            if(distanceToPlayer <= chaseDistance && distanceToPlayer > attackDistance)
            {
                Chasing();
            }
            else if (distanceToPlayer <= attackDistance)
            {
                Attacking();
            }
            else
            {
                isPatrolling = true; // Resume patrolling when out of range
                canChaseAndAttack = false; // Stop chasing when not in range
                StartPatrol();
        }
        
    }

    void Chasing()
    {
        isPatrolling = false; // Stop patrolling when chasing
        canChaseAndAttack = true; // Enable chasing and attacking
        animator.SetBool("isAttacking", false);
        agent.SetDestination(player.transform.position); // Set the destination to the player's position
        transform.LookAt(player.transform.position); // Rotate to face the player
       
    }
    void Attacking()
    {
        // Implement attack logic here
        Debug.Log("Attacking the player!");
        agent.SetDestination(transform.position); // Stop moving when attacking
        transform.LookAt(player.transform);
        animator.SetBool("isAttacking", true);
    }
}
