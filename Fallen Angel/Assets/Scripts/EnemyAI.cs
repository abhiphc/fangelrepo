using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float distanceToPlayer;
    [SerializeField] float chaseDistance = 10f;
    GameObject player;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= chaseDistance)
        {
            agent.SetDestination(player.transform.position); // Set the destination to the player's position
            transform.LookAt(player.transform.position); // Rotate to face the player
        }
        else
        {
            agent.SetDestination(transform.position); // Stop moving when out of range
        }
    }
}
