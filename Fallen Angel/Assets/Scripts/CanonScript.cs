using UnityEngine;

public class CanonScript : MonoBehaviour
{
    [SerializeField] GameObject blastEffect;
    [SerializeField] float blastDuration = 1.5f;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag + " Hits");
        if (collision.gameObject.tag == "Obstacle")
        {
            GameObject blast = Instantiate(blastEffect, transform.position, Quaternion.identity);
            Destroy(blast, blastDuration);
            Destroy(collision.gameObject);
            Destroy(gameObject,1f);
        }
        else if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Hit");
            //collision.gameObject.GetComponent<PlayerController>().Die();

        }
        else
        {
            GameObject blast = Instantiate(blastEffect, transform.position, Quaternion.identity);
            Destroy(blast, blastDuration);
            Destroy(gameObject,1f);

        }
    }
    
}
