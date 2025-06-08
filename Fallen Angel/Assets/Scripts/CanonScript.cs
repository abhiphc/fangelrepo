using UnityEngine;

public class CanonScript : MonoBehaviour
{
    GameObject pl;
    [SerializeField] float damageAmount = 20f;

    private void Start()
    {
        pl = GameObject.FindGameObjectWithTag("Player");
    }
    void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Hits");
            pl.GetComponent<PlayerHealthController>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        
    }
    
}
