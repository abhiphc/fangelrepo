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
    [Header("Enemy Gun Properties")]
    [SerializeField] bool isEnemyFiring =false;
    [SerializeField] AudioSource gunFireSound;
    [SerializeField] GameObject bulletPointObj;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed = 100f;
    [SerializeField] GameObject muzzleFlash;

    [Header("Enemy Type")]
    [SerializeField] bool isNonPetrolEnemy = false;

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
        if (isPatrolling && !isNonPetrolEnemy)
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

        if (isPatrolling && isNonPetrolEnemy)
        {
            agent.SetDestination(transform.position);
        }
        
    }
    void ChasingAndAttacking()
    {
        if(!isNonPetrolEnemy)
        {
            if (distanceToPlayer <= chaseDistance && distanceToPlayer > attackDistance)
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
        if(isNonPetrolEnemy)
        {
            if(distanceToPlayer <= attackDistance)
            {
                Attacking();
            }
            else
            {
                isPatrolling = true; 
                canChaseAndAttack = false;
                StartPatrol();
            }
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

        if(!isNonPetrolEnemy)
        {
            animator.SetBool("isAttacking", true);
        }
        
        if(!isEnemyFiring)
        {
            StartCoroutine(EnemyGunFiring());
        }
        
    }

    IEnumerator EnemyGunFiring()
    {
        isEnemyFiring = true;
        muzzleFlash.SetActive(true);
        gunFireSound.Play();
        GameObject go = Instantiate(bullet, bulletPointObj.transform.position, Quaternion.identity);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if(rb !=null)
        {
            rb.linearVelocity = bulletPointObj.transform.forward * bulletSpeed * Time.deltaTime;
        }
        Destroy(go, 5f);
        yield return new WaitForSeconds(0.25f);
        muzzleFlash.SetActive(false);
        yield return new WaitForSeconds(1f);
        isEnemyFiring = false;
    }
}
