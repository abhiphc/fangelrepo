using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using StarterAssets;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider healthbar;
    [SerializeField] float damageAmount;
    private float health;
    public static bool isDead;
    [SerializeField] Animator playerAnimator;


    private void Start()
    {
        health = maxHealth;
        isDead = false;
        this.GetComponent<ThirdPersonController>().enabled = true;
        this.GetComponent<CharacterController>().enabled = true;
    }

    void Update()
    {
        //TakeDamage(damageAmount);
        //HealthRegain();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthbar.value -= damage;
        if (health <= 0f)
        {
            Die();
        }
        HealthRegain();

        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        health -= damage;
        //        healthbar.value -= damage;
        //        if (health <= 0f)
        //        {
        //            Die();
        //        }
        //        HealthRegain();
        //    }
        //}
    }
    void Die()
    {
        Debug.Log("Player dead.");
        isDead = true;
        playerAnimator.SetTrigger("isDieing");
        this.GetComponent<ThirdPersonController>().enabled = false;
        this.GetComponent<CharacterController>().enabled = false;

    }
    void HealthRegain()
    {
        if((health < maxHealth) && !isDead)
        {
            health += Time.deltaTime * 4f;
            if(health >= maxHealth)
            {
                health = maxHealth;
            }
        }
        healthbar.value = health;
    }
}
