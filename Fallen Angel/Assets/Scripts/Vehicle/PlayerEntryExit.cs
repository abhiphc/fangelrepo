using UnityEngine;
using StarterAssets;
public class PlayerEntryExit : MonoBehaviour
{
    [SerializeField] GameObject car;
    [SerializeField] GameObject player;
    RCC_CarControllerV3 carController;
    [SerializeField] GameObject carCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField] bool canRide = false;
    private void Awake()
    {
        carController = car.GetComponent<RCC_CarControllerV3>();
        carController.enabled = false;
        carCamera.GetComponent<RCC_Camera>().enabled = false;
        carCamera.SetActive(false);
    }
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CarEnterExit();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            canRide = true;
        }
        else
        {
            canRide = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canRide = false;
    }

    void CarEnterExit()
    {
        
            if (canRide)
            {
                canRide = false;
                player.GetComponent<CharacterController>().enabled = false;
                player.GetComponent<ThirdPersonController>().enabled = false;
                player.GetComponent<BasicRigidBodyPush>().enabled = false;
                player.GetComponent<StarterAssetsInputs>().enabled = false;
                player.GetComponent<PlayerGameMech>().enabled = false;
                player.GetComponent<WeaponSelector>().enabled = false;
                player.SetActive(false);

                carController.enabled = true;
                carCamera.GetComponent<RCC_Camera>().enabled = true;
                carCamera.SetActive(true);
                mainCamera.SetActive(false);

            }
            else
            {
                canRide = true;
                player.SetActive(true);
                player.transform.position = transform.position;
                player.GetComponent<CharacterController>().enabled = true;
                player.GetComponent<ThirdPersonController>().enabled = true;
                player.GetComponent<BasicRigidBodyPush>().enabled = true;
                player.GetComponent<StarterAssetsInputs>().enabled = true;
                player.GetComponent<PlayerGameMech>().enabled = true;
                player.GetComponent<WeaponSelector>().enabled = true;
                
                
                carController.enabled = false;
                carCamera.GetComponent<RCC_Camera>().enabled = false;
                carCamera.SetActive(false);
                mainCamera.SetActive(true);

            }

    }
}

