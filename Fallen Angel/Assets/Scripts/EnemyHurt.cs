using UnityEngine;

public class EnemyHurt : MonoBehaviour
{
    [Header("Enemy Health Properties")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] bool isEnemyDead;
    [SerializeField] float currentHealth;
    Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        isEnemyDead = false;
        animator = GetComponent<Animator>();
    }

    public void HarmEnemy(float harmValue)
    {
        currentHealth -= harmValue;
        if(currentHealth <= 0f && !isEnemyDead)
        {
            //Enemy dies;
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Enemy Dead");
        isEnemyDead = true;
        if(animator!=null)
        {
            animator.SetTrigger("EnemyDies");
            GetComponent<EnemyAI>().enabled = false;
            Destroy(gameObject, 5f);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

}