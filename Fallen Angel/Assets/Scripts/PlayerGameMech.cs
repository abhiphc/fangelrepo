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
    [SerializeField] List<Transform> holsterWpn = new List<Transform>();
    [SerializeField] LineRenderer bulletRenderer;
    [SerializeField] GameObject bulletRndObj;
    [SerializeField] Rig rig1;
    [SerializeField] float bulletForce = 1000f;

    [Header("Bullet Effects")]
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject bulletImpactBlood;

    RaycastHit hit;
    [SerializeField] List<Transform> weapons = new List<Transform>();

    [Header("Shotgun Properties")]
    [SerializeField] GameObject muzzleFlashSG;
    [SerializeField] LineRenderer bulletRendererSG;
    [SerializeField] GameObject bulletRndObjSG;

    [Header("Sniper Properties")]
    [SerializeField] GameObject sniperScope;
    [SerializeField] float sniperCameraFOV = 10f;
    private float normalAimFOV;
    [SerializeField] bool isScoped = false;
    [SerializeField] bool isSniperGun = false;

    [Header("RPG Properties")]
    [SerializeField] GameObject muzzleFlashRPG;
    [SerializeField] GameObject blastEffect;
    [SerializeField] float cannonFireDelay = 2f;
    [SerializeField] float blastForceUp = 1000f;
    [SerializeField] float blastRadius = 5f;
    [SerializeField] LineRenderer bulletRendererRPG;
    [SerializeField] GameObject bulletRndObjRPG;

    [Header("Game Audios")]
    [SerializeField] AudioSource gunFire;
    [SerializeField] AudioSource shotgunFire;
    [SerializeField] AudioSource rpgFire;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        akHandler.SetActive(false);
        aimObj.SetActive(false);
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
            holsterWpn[WeaponSelector.selectedWeaponIndex].gameObject.SetActive(false); //dummy AK is not activated when aiming
            VCameras[0].gameObject.SetActive(false);
            VCameras[1].gameObject.SetActive(true);
            crossbar.SetActive(true);
            this.GetComponent<ThirdPersonController>().canRotate = false;
            this.GetComponent<ThirdPersonController>().sensitivity = aimSensitivity;
            PlayerAimDirection();
            //aiming mode on for Weapons
            animator.SetLayerWeight(3, 1); //Enable aiming layer
           
            WeaponSelect(); // Select weapon through WeaponSelector script

            //Sniper Gun scoped
            if (isSniperGun && !isScoped)
            {
                isScoped = true;
                sniperScope.SetActive(true);
                normalAimFOV = VCameras[1].m_Lens.FieldOfView;
                VCameras[1].m_Lens.FieldOfView = sniperCameraFOV;
            }
            
            //firing guns by left clicking mouse
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
                    StartCoroutine(ShootSG(hit)); //for shooting Shotgun
                }
                if (WeaponSelector.selectedWeaponIndex == 2)
                {
                    Debug.Log("Sniper is firing");
                    StartCoroutine(ShootSniper(hit)); //for shooting Sniper
                }
               
            }

            if (Input.GetMouseButtonDown(0) && !isShooting)
            {
                if (WeaponSelector.selectedWeaponIndex == 3)
                {
                    Debug.Log("RPG is firing");
                    StartCoroutine(ShootRPG(hit)); //for shooting RPG
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
            holsterWpn[WeaponSelector.selectedWeaponIndex].gameObject.SetActive(true);
            VCameras[0].gameObject.SetActive(true);
            VCameras[1].gameObject.SetActive(false);
            crossbar.SetActive(false);
            this.GetComponent<ThirdPersonController>().canRotate = true;
            this.GetComponent<ThirdPersonController>().sensitivity = normalSensitivity;
            //aiming mode off
            animator.SetLayerWeight(3, 0);

            //Sniper un-scoped
            if (isSniperGun && isScoped)
            {
                isScoped = false;
                sniperScope.SetActive(false);
                VCameras[1].m_Lens.FieldOfView = normalAimFOV;
            }
            isScoped = false;
            sniperScope.SetActive(false);
            VCameras[1].m_Lens.FieldOfView = 18f;
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
        //Fire sound
        gunFire.Play();
        
        bulletRenderer.enabled = true;
        bulletRenderer.SetPosition(0, bulletRndObj.transform.position);
        bulletRenderer.SetPosition(1, hit.point);

        //Harm Enemy with AK
        if (hit.collider.tag == "Enemy" || hit.collider.tag == "EnemyHead")
        {
            GameObject bulletImpactObjEnemy = Instantiate(bulletImpactBlood, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(bulletImpactObjEnemy, 1.5f);
            
                if (hit.collider.tag == "Enemy")
                {
                    EnemyHurt enemyHurt = hit.transform.GetComponent<EnemyHurt>();
                    if (enemyHurt != null)
                    {
                        enemyHurt.HarmEnemy(20f);
                    }
                
                }
                else if(hit.collider.tag == "EnemyHead")
                {
                    EnemyHurt enemyHurt = hit.transform.GetComponentInParent<EnemyHurt>();
                    if (enemyHurt != null)
                    {
                        enemyHurt.HarmEnemy(100f);
                    }
                }
               
            
        }
        else
        {
            //**** bullet effects AK for ofstacles;
            GameObject bulletImpactObj = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(bulletImpactObj, 1.5f);
        }
        
        if (hit.rigidbody != null && hit.collider.tag != "Player")
        {
            hit.rigidbody.AddForce(-hit.normal * bulletForce);
        }
        //****
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
        bulletRenderer.enabled = false;
        yield return new WaitForSeconds(0.01f);
        isShooting = false;

    }
    // Shooting Shotgun Coroutine
    IEnumerator ShootSG(RaycastHit hit)
    {
        isShooting = true;
        muzzleFlashSG.SetActive(true);
        //Fire sound

        shotgunFire.Play();
        

        bulletRendererSG.enabled = true;
        bulletRendererSG.SetPosition(0, bulletRndObjSG.transform.position);
        bulletRendererSG.SetPosition(1, hit.point);

        //Harm Enemy with ShotGun
        if (hit.collider.tag == "Enemy" || hit.collider.tag == "EnemyHead")
        {
            GameObject bulletImpactObjEnemy = Instantiate(bulletImpactBlood, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(bulletImpactObjEnemy, 1.5f);

            if (hit.collider.tag == "Enemy")
            {
                EnemyHurt enemyHurt = hit.transform.GetComponent<EnemyHurt>();
                if (enemyHurt != null)
                {
                    enemyHurt.HarmEnemy(30f);
                }

            }
            else if (hit.collider.tag == "EnemyHead")
            {
                EnemyHurt enemyHurt = hit.transform.GetComponentInParent<EnemyHurt>();
                if (enemyHurt != null)
                {
                    enemyHurt.HarmEnemy(100f);
                }
            }
        }
        else
        {
            //**** bullet effects ShotGun for ofstacles;
            GameObject bulletImpactObj = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(bulletImpactObj, 1.5f);
        }
       
        if (hit.rigidbody != null && hit.collider.tag != "Player")
        {
            hit.rigidbody.AddForce(-hit.normal * bulletForce);
        }
        //****
        yield return new WaitForSeconds(0.15f);
        muzzleFlashSG.SetActive(false);
        bulletRendererSG.enabled = false;
        yield return new WaitForSeconds(0.5f);
        isShooting = false;

    }

    //Shooting Sniper Coroutine
    IEnumerator ShootSniper(RaycastHit hit)
    {
        isShooting = true;
        //Fire sound
        gunFire.Play();

        //Harm Enemy with Sniper
        if (hit.collider.tag == "Enemy" || hit.collider.tag == "EnemyHead")
        {
            GameObject bulletImpactObjEnemy = Instantiate(bulletImpactBlood, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(bulletImpactObjEnemy, 1.5f);

            if (hit.collider.tag == "Enemy")
            {
                EnemyHurt enemyHurt = hit.transform.GetComponent<EnemyHurt>();
                if (enemyHurt != null)
                {
                    enemyHurt.HarmEnemy(50f);
                }

            }
            else if (hit.collider.tag == "EnemyHead")
            {
                EnemyHurt enemyHurt = hit.transform.GetComponentInParent<EnemyHurt>();
                if (enemyHurt != null)
                {
                    enemyHurt.HarmEnemy(100f);
                }
            }
        }
        else
        {
            //**** bullet effects Sniper for obstacles;
            GameObject bulletImpactObj = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(bulletImpactObj, 1.5f);
        }

        if (hit.rigidbody != null && hit.collider.tag != "Player")
        {
            hit.rigidbody.AddForce(-hit.normal * bulletForce);
        }

        yield return new WaitForSeconds(0.5f);
        isShooting = false;
    }

    // Shooting RPG Coroutine
    IEnumerator ShootRPG(RaycastHit hit)
    {
        //isShooting = true;
        //**** Spawn and Fire Cannon from RPG
        //GameObject cannon = Instantiate(cannonPrefab, cannonSpawnObj.transform.position,Quaternion.identity);
        //cannon.GetComponent<Rigidbody>().AddForce(cannonSpawnObj.transform.forward * cannonForce, ForceMode.VelocityChange);
        //muzzleFlashRPG.SetActive(true);
        ////Destroy(cannon, 5f);
        //yield return new WaitForSeconds(0.5f);
        //muzzleFlashRPG.SetActive(false);
        ////bulletRendererSG.enabled = false;
        //yield return new WaitForSeconds(cannonFireDelay);
        //isShooting = false;

        isShooting = true;
        muzzleFlashRPG.SetActive(true);
        //blast sound
        rpgFire.Play();
        
        bulletRendererRPG.enabled = true;
        bulletRendererRPG.SetPosition(0, bulletRndObjRPG.transform.position);
        bulletRendererRPG.SetPosition(1, hit.point);
        //**** Blast effect
        GameObject bEffect = Instantiate(blastEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(bEffect, 1.5f);
        //if (hit.rigidbody != null && hit.collider.tag != "Player")
        //{
        //    hit.rigidbody.AddForce(Vector3.up * blastForceUp);
        //}
            Collider[] colliders = Physics.OverlapSphere(hit.point, 5f);
                foreach (Collider collider in colliders)
                {
                        Rigidbody rb = collider.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.AddExplosionForce(blastForceUp, hit.point, blastRadius);
                        }
                        if(collider.tag == "Enemy")
                        {
                            Destroy(collider.gameObject);
                        }
                }
        yield return new WaitForSeconds(0.25f);
        bulletRendererRPG.enabled = false;
        yield return new WaitForSeconds(0.5f);
        muzzleFlashRPG.SetActive(false);
        yield return new WaitForSeconds(cannonFireDelay);
        isShooting = false;
    }

    //Select weapon
    void WeaponSelect()
    {
        if(WeaponSelector.selectedWeaponIndex == 0) //AK
        {
            weapons[0].gameObject.SetActive(true);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(false);
            isSniperGun = false;
        }

        if (WeaponSelector.selectedWeaponIndex == 1) //Shotgun
        {
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(true);
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(false);
            isSniperGun = false;
        }

        if (WeaponSelector.selectedWeaponIndex == 2) //Sniper
        {
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(true);
            weapons[3].gameObject.SetActive(false);
            isSniperGun = true;
        }
        if(WeaponSelector.selectedWeaponIndex == 3) //RPG
        {
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(true);
            isSniperGun = false;
        }   
    }
    
}
