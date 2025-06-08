using UnityEngine;

public class EnemyHarmCombat : MonoBehaviour
{
    [SerializeField] AudioSource kickPunchAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "EnemyHead")
        {
            EnemyHurt enemyHurt = other.GetComponent<EnemyHurt>();
            if(enemyHurt !=null)
            {
                kickPunchAudio.Play();
                enemyHurt.HarmEnemy(5f);
                Debug.Log("You hit enemy through combat");
            }

        }
    }
}
