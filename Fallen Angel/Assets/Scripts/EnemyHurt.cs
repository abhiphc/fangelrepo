using UnityEngine;

public class EnemyHurt : MonoBehaviour
{
    [Header("Enemy Health Properties")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] bool isEnemyDead;
    [SerializeField] float currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
        isEnemyDead = false;
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
        Destroy(gameObject);
    }

}