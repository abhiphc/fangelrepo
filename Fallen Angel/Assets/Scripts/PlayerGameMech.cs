using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class PlayerGameMech : MonoBehaviour
{
    public bool isAiming;
    public bool isInCombat;
    public static bool hasGun;
    [SerializeField] GameObject crossbar;
    [SerializeField] List<CinemachineVirtualCamera> VCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] List<GameObject> hitboxes = new List<GameObject>();
    [SerializeField] float normalSensitivity;
    [SerializeField] float aimSensitivity;
    private Animator animator;
    [SerializeField] GameObject akHandler;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] bool isShooting = false;
    [SerializeField] GameObject aimObj;
    //[SerializeField] GameObject dummyAK;
    [SerializeField] LineRenderer bulletRenderer;
    [SerializeField] GameObject bulletRndObj;
    [SerializeField] Rig rig1;
    [SerializeField] float bulletForce = 1000f;
    [SerializeField] GameObject bulletImpact;
    RaycastHit hit;
    [SerializeField] List<Transform> weapons = new List<Transform>();
    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        akHandler.SetActive(false);
        aimObj.SetActive(false);
        //dummyAK.SetActive(true);
        crossbar.SetActive(false);
        rig1.weight = 0f; // Disable the rig at the start
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.SetActive(false);
        }
    }
    void Start()
    {
        hasGun = false;
        isAiming = false;
        isInCombat = true; 
    }

    void Update()
    {
        if (hasGun)
        {
            AimAndShoot();
        }
        Combat();
    }

    void AimAndShoot()
    {
        if (Input.GetMouseButton(1) && !PlayerHealthController.isDead)
        {
            isAiming = true;
            //isInCombat = false; // Disable combat mode when aiming
            rig1.weight = 1f; // Enable the rig when aiming
            aimObj.SetActive(true);
            akHandler.SetActive(true);
            //dummyAK.SetActive(false); //dummy AK is not activated when aiming
            VCameras[0].gameObject.SetActive(false);
            VCameras[1].gameObject.SetActive(true);
            crossbar.SetActive(true);
            this.GetComponent<ThirdPersonController>().canRotate = false;
            this.GetComponent<ThirdPersonController>().sensitivity = aimSensitivity;
            PlayerAimDirection();
            //aiming mode on for AK
            animator.SetLayerWeight(3, 1); //Enable aiming layer

            WeaponSelect(); // Select weapon through WeaponSelector script

            
            if (Input.GetMouseButton(0) && !isShooting)
            {
                if(WeaponSelector.selectedWeaponIndex == 0)
                {
                    Debug.Log("AK is firing");
                    StartCoroutine(ShootAK(hit)); //for shooting AK
                }
                if (WeaponSelector.selectedWeaponIndex == 1)
                {
                    Debug.Log("Shotgun is firing");
                    StartCoroutine(ShootAK(hit)); //for shooting Shotgun
                }
                if (WeaponSelector.selectedWeaponIndex == 2)
                {
                    Debug.Log("Sniper is firing");
                    StartCoroutine(ShootAK(hit)); //for shooting Sniper
                }
                if (WeaponSelector.selectedWeaponIndex == 3)
                {
                    Debug.Log("RPG is firing");
                    StartCoroutine(ShootAK(hit)); //for shooting RPG
                }

            }
        }
        else
        {
            isAiming = false;
            //isInCombat = true; // Enable combat mode when not aiming
            rig1.weight = 0f; // Disable the rig when not aiming
            akHandler.SetActive(false);
            aimObj.SetActive(false);
            //dummyAK.SetActive(true); //dummy AK is activated when not aiming
            VCameras[0].gameObject.SetActive(true);
            VCameras[1].gameObject.SetActive(false);
            crossbar.SetActive(false);
            this.GetComponent<ThirdPersonController>().canRotate = true;
            this.GetComponent<ThirdPersonController>().sensitivity = normalSensitivity;
            //aiming mode off
            animator.SetLayerWeight(3, 0);
        }
    }

    void Combat() // For Combat Mode Action
    {
        if(Input.GetMouseButtonDown(0) && isInCombat && !isAiming && !PlayerHealthController.isDead)
        {
            int combatAction = Random.Range(0, 2); // Randomly choose between 0 and 2 for combat types
            if(combatAction == 0)
            {
                StartCoroutine(CombatPunch());
            }
            else
            {
                StartCoroutine(CombatKick());
            }
        }
    }
    IEnumerator CombatPunch()
    {
        isInCombat = false;
        animator.SetLayerWeight(2, 0); // Disable combat Kick layer
        animator.SetBool("isKicking", false);
        animator.SetLayerWeight(1, 1); // Enable combat Punch layer
        animator.SetBool("isPunching", true);
        hitboxes[0].SetActive(true); // Enable punch hitbox
        hitboxes[1].SetActive(true);
        yield return new WaitForSeconds(1.15f);
        animator.SetLayerWeight(1, 0); // Disable combat Punch layer
        animator.SetBool("isPunching", false);
        hitboxes[0].SetActive(false); // Disble punch hitbox
        hitboxes[1].SetActive(false);
        yield return new WaitForSeconds(0.25f);
        isInCombat = true;
    }
    IEnumerator CombatKick()
    {
        isInCombat = false;
        animator.SetLayerWeight(1, 0); // Disable combat Punch layer
        animator.SetBool("isPunching", false);
        animator.SetLayerWeight(2, 1); // Enable combat Kick layer
        animator.SetBool("isKicking", true);
        hitboxes[2].SetActive(true); //Enable kick hitbox
        yield return new WaitForSeconds(1.15f);
        animator.SetLayerWeight(2, 0); // Disable combat Kick layer
        animator.SetBool("isKicking", false);
        hitboxes[2].SetActive(false);
        yield return new WaitForSeconds(0.25f);

        isInCombat = true;
    }

    void PlayerAimDirection()
    {
        Vector3 aimDirection = Vector3.zero;
        Vector3 worldAimPoint = Vector3.zero;
        Vector2 cntrOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(cntrOfScreen);
        if (Physics.Raycast(ray, out hit, 999f))
        {
            worldAimPoint = hit.point;
            worldAimPoint.y = transform.position.y;
            aimDirection = (worldAimPoint - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            aimObj.transform.position = hit.point;

        }

    }
    //Shooting AK Coroutine
    IEnumerator ShootAK(RaycastHit hit)
    {
        isShooting = true;
        muzzleFlash.SetActive(true);
        bulletRenderer.enabled = true;
        bulletRenderer.SetPosition(0, bulletRndObj.transform.position);
        bulletRenderer.SetPosition(1, hit.point);
        //**** bullet effects AK
        GameObject bulletImpactObj = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(bulletImpactObj, 1.5f);
        if (hit.rigidbody != null && hit.collider.tag != "Player")
        {
            hit.rigidbody.AddForce(-hit.normal * bulletForce);
        }
        //****
        yield return new WaitForSeconds(0.15f);
        muzzleFlash.SetActive(false);
        bulletRenderer.enabled = false;
        yield return new WaitForSeconds(0.01f);
        isShooting = false;

    }

    //Select weapon
    void WeaponSelect()
    {
        if(WeaponSelector.selectedWeaponIndex == 0)
        {
            weapons[0].gameObject.SetActive(true);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(false);
        }

        if (WeaponSelector.selectedWeaponIndex == 1)
        {
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(true);
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(false);
        }

        if (WeaponSelector.selectedWeaponIndex == 2)
        {
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(true);
            weapons[3].gameObject.SetActive(false);
        }
        if(WeaponSelector.selectedWeaponIndex == 3)
        {
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(true);
        }   
    }
    
}
