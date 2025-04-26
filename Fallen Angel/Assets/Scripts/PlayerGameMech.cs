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
    [SerializeField] GameObject crossbar;
    [SerializeField] List<CinemachineVirtualCamera> VCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] float normalSensitivity;
    [SerializeField] float aimSensitivity;
    private Animator animator;
    [SerializeField] GameObject akHandler;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] bool isShooting = false;
    [SerializeField] GameObject aimObj;
    [SerializeField] GameObject dummyAK;
    [SerializeField] LineRenderer bulletRenderer;
    [SerializeField] GameObject bulletRndObj;
    [SerializeField] Rig rig1;
    [SerializeField] float bulletForce = 1000f;
    [SerializeField] GameObject bulletImpact;
    RaycastHit hit;
    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        akHandler.SetActive(false);
        aimObj.SetActive(false);
        dummyAK.SetActive(true);
        rig1.weight = 0f; // Disable the rig at the start
    }
    void Start()
    {
        isAiming = false;
        isInCombat = true; 
    }

    void Update()
    {
        AimAndShoot();
        Combat();
        
    }

    void AimAndShoot()
    {
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            isInCombat = false; // Disable combat mode when aiming
            rig1.weight = 1f; // Enable the rig when aiming
            aimObj.SetActive(true);
            akHandler.SetActive(true);
            dummyAK.SetActive(false); //dummy AK is not activated when aiming
            VCameras[0].gameObject.SetActive(false);
            VCameras[1].gameObject.SetActive(true);
            crossbar.SetActive(true);
            this.GetComponent<ThirdPersonController>().canRotate = false;
            this.GetComponent<ThirdPersonController>().sensitivity = aimSensitivity;
            PlayerAimDirection();
            //aiming mode on for AK
            animator.SetLayerWeight(3, 1); //Enable aiming layer

            //for shooting AK
            if (Input.GetMouseButton(0) && !isShooting)
            {
                StartCoroutine(ShootAK(hit));
            }


        }
        else
        {
            isAiming = false;
            isInCombat = true; // Enable combat mode when not aiming
            rig1.weight = 0f; // Disable the rig when not aiming
            akHandler.SetActive(false);
            aimObj.SetActive(false);
            dummyAK.SetActive(true); //dummy AK is activated when not aiming
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
        if(Input.GetMouseButtonDown(0) && isInCombat && !isAiming)
        {
            isInCombat = false;
            StartCoroutine(CombatPunch());
            
        }
       
    }

    IEnumerator CombatPunch()
    {
        
        animator.SetLayerWeight(1, 1); // Enable combat Punch layer
        animator.SetBool("isPunching", true);
        yield return new WaitForSeconds(1.15f);
        animator.SetLayerWeight(1, 0); // Disable combat Punch layer
        animator.SetBool("isPunching", false);
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
    
}
