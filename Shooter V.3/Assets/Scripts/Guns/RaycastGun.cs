using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RaycastGun : MonoBehaviour
{
    [Header("Damage")]
    public int minDamage = 2;
    public int maxDamage = 5;
    [Range(0f, 100f)]
    public int criticalStrikeChance = 10;
    public GameObject explosion;

    [Header("Draw Ray")]
    public bool drawBulletRay = false;
    public float bulletRayLiveTime = 0.1f;
    public Transform lineRenderStartLocation;
    public GameObject bulletRay;

    [Header("Range & Delay Between Shots")]
    public float range = 100f;
    public float shotDelay = 0.1f;

    [Header("Reload & Ammo")]
    public int maxAmmo = 10;
    public bool usesAmmo = true;
    public float reloadTime = 1.5f;
    public bool reloadBulletsIndividual = false;
    public bool shootToInteruptReload = false;

    [Header("Automatic Fire")]
    public bool isAutoFire = false;

    [Header("Burst Fire")]
    public bool isBurstFire = false;
    public bool oneBulletPerBurst = false;
    public int burstAmount = 3;
    public float delayBetweenBursts = 0.1f;

    [Header("Shotgun Fire")]
    public int bulletsPerShot = 1;

    [Header("Bullet Spread")]
    public float bloom = 10f;

    [Header("Weapon Sway")]
    public float swayAmount = 0.06f;
    public float smoothAmount = 6;
    public float maxSwayAmount = 0.12f;

    [Header("Weapon Aim")]
    public GameObject crosshair;
    public bool isAimable = true;
    public float aimFovChange = 10.0f;
    public float aimFovSpeed = 0.1f;

    [Header("Knockback")]
    public float knockbackForce = 500f;

    [Header("Objects")]
    public Text ammoUI;
    public Image gunIcon;
    public GameObject fpsCam;

    [Header("Audio")]
    public GameObject shootSound;
    public GameObject reloadSound;
    public GameObject reloadFinishedSound;
    public GameObject shotDelaySound;
    public float timeUntilDelaySoundPlayed = 0.3f;

    [HideInInspector]
    public float shotVolume = 1f;
    [HideInInspector]
    public float shotPitch = 1f;

    [Header("Particles")]
    public ParticleSystem muzzleFlash;
    public LayerMask cannotBeShot;

    [Header("Bulletholes")]
    public bool doBulletHoles = true;
    public float bulletHoleLiveTime = 30f;
    public GameObject bulletHole;
    public GameObject incendiaryBulletHole;

    [Header("Recoil")]
    public float rotationSpeed = 6;
    public float returnSpeed = 25;
    public Vector3 recoilRotation = new Vector3(2f, 2f, 2f);

    [HideInInspector]
    public bool isReloading = false;

    Vector3 initialPosition;
    float reloadDuration;
    float initialBloom;
    float currentShotDelay;
    float currentBurstDelay = 0;
    int ammo;
    bool isAiming;
    bool isDelaying = false;
    bool isBursting = false;
    int currentBurstAmount;
    bool playedDelaySound;
    Interact playerInteract;
    PlayerMovement playerMove;
    aimingFov aimingFov;

    void Start()
    {
        AssignStartVariables();
    }

    void OnDisable()
    {
        //this prevents reloading from occuring when the gun isn't being held.
        isReloading = false;
    }

    void Update()
    {
        AimAndShootInput();
        UI();
        WeaponSway();
    }

    void FixedUpdate()
    {
        UpdateTracker();
    }

    void AimAndShootInput()
    {
        //Checks that the player isnt holding anything
        if (!playerInteract.carrying)
        {
            ShootInput();

            //statement 1: if the gun is aimable, statement 2: if the playerMove script is allowing the gun to aim, statement 3: if the gun is not reloading.
            if (isAimable && playerMove.canAim && !isReloading)
            {
                GunAim();
            }
            //if the player is not allowed to aim revert all aiming operations.
            else
            {
                NotAiming();
            }
        }
        //if the player is carrying something, revert all aiming operations.
        else
        {
            NotAiming();
        }
    }

    void NotAiming()
    {
        if(isAiming)
            isAiming = false;

        if (aimingFov.isAiming)
            aimingFov.isAiming = false;
    }

    void AssignStartVariables()
    {
        //this is for weapon sway
        initialPosition = transform.localPosition;

        //this is for weapon bloom
        initialBloom = bloom;

        playerInteract = GameObject.Find("Player").GetComponent<Interact>();
        playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
        aimingFov = fpsCam.GetComponent<aimingFov>();
    }

    void ShootInput()
    {
            if (Input.GetButtonDown("Reload"))
            {
                Reload();
            }

            if (!isAutoFire)
            {
                if (Input.GetButtonDown("Fire1") && ammo > 0 && !isReloading && !isDelaying && !isBursting)
                {
                    if (isBurstFire)
                    {
                        BurstShot();
                    }
                    else
                    {
                        SingleShot();
                    }
                }
                else if (Input.GetButtonDown("Fire1") && ammo <= 0)
                {
                    Reload();
                }
                else if (Input.GetButtonDown("Fire1") && isReloading && shootToInteruptReload && ammo > 0)
                {
                    isReloading = false;

                    if (isBurstFire)
                    {
                        BurstShot();
                    }
                    else
                    {
                        SingleShot();
                    }
                }
            }
            else
            {
                if (Input.GetButton("Fire1") && ammo > 0 && !isReloading && !isDelaying && !isBursting)
                {
                    if (isBurstFire)
                    {
                        BurstShot();
                    }
                    else
                    {
                        SingleShot();
                    }
                }
                else if (Input.GetButton("Fire1") && ammo <= 0 && !isReloading && !isDelaying)
                {
                    Reload();
                }
                else if (Input.GetButton("Fire1") && isReloading && shootToInteruptReload && ammo > 0)
                {
                    isReloading = false;

                    //checks for burst fire, executes scripts accordingly
                    if (isBurstFire)
                    {
                        BurstShot();
                    }
                    else
                    {
                        SingleShot();
                    }
                }
            }
    }

    void BurstShot()
    {
        BurstShoot();
        ShootDelay();
        if (oneBulletPerBurst && usesAmmo)
            ammo--;
    }

    void SingleShot()
    {
        Shoot();
        ShootDelay();
        if (usesAmmo)
            ammo--;
    }


    void UpdateTracker()
    {
        //this shoots the burst shot
        if (isBursting)
        {
            if (currentBurstDelay <= Time.time && currentBurstAmount > 0)
            {
                currentBurstDelay = Time.time + delayBetweenBursts;
                currentBurstAmount--;
                Shoot();
                if (oneBulletPerBurst == false && usesAmmo)
                    ammo--;
            }
            else if (currentBurstAmount == 0)
            {
                isBursting = false;
            }
        }

        // Below checks for the delay between shots, handles rapid fire and non rapid fire
        if (isDelaying)
        {
            if (currentShotDelay <= Time.time)
            {
                isDelaying = false;

                if(playedDelaySound)
                {
                    playedDelaySound = false;
                }
            }
        }

        //this checks whether the reloading is finished and reloads bullets accordingly
        if (isReloading)
        {
            //***************- This disable the werid crosshair bug -***************\\
            crosshair.GetComponent<Reticle>().aiming = false;

            if (!reloadBulletsIndividual)
            {
                ammoUI.text = "...  / " + maxAmmo;
                //formula below basically divides the time 3 seconds ago by ther current time and applies it to a image filler.
                gunIcon.fillAmount = (reloadDuration - Time.time) / reloadTime;
            }

            if (reloadDuration <= Time.time)
            {
                if (!reloadBulletsIndividual)
                {
                    ammo = maxAmmo;
                    isReloading = false;

                    var audio = Instantiate(reloadFinishedSound, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }
                else
                {
                    reloadDuration = Time.time + reloadTime;
                    ammo++;
                    if(ammo != maxAmmo)
                    {
                        var audio = Instantiate(reloadSound, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                        audio.transform.parent = GameObject.Find("AudioHolder").transform;
                    }

                }
            }

            if (reloadBulletsIndividual && ammo == maxAmmo)
            {
                isReloading = false;

                var audio = Instantiate(reloadFinishedSound, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                audio.transform.parent = GameObject.Find("AudioHolder").transform;
            }
        }
    }

    void Shoot()
    {
        //The function below detects whether the gun is a shotgun, shoots accordingly
        for(int i = 0; i < Mathf.Max(1, bulletsPerShot); i++)
        {
            //this handles bloom
            Vector3 t_bloom = fpsCam.transform.position + fpsCam.transform.forward * 1000f;
            t_bloom += UnityEngine.Random.Range(-bloom, bloom) * fpsCam.transform.up;
            t_bloom += UnityEngine.Random.Range(-bloom, bloom) * fpsCam.transform.right;
            t_bloom -= fpsCam.transform.position;
            t_bloom.Normalize();

            //this shoots the beam /bullet
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.GetComponent<Camera>().transform.position, t_bloom, out hit, range, ~cannotBeShot))
            {
                Target target = hit.transform.GetComponent<Target>();

                //***************************************************************************\\
                //                                                                           \\
                //                      Handles Damage for Non-Explosions                    \\
                //                                   \/                                      \\
                //***************************************************************************\\

                if (target != null && explosion == null)
                {
                    float damage;
                    bool isCritical;

                    if (UnityEngine.Random.Range(0, 100) <= criticalStrikeChance)
                    {
                        damage = maxDamage * 2;
                        isCritical = true;
                    }
                    else
                    {
                        //I'm doing + 1 after maxDamage because Random.Range's never grab the max value.
                        damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                        isCritical = false;
                    }

                     target.HitPosition(hit.point);
                     target.AssignColor(minDamage, maxDamage, isCritical);
                     target.TakeDamage(Mathf.RoundToInt(damage));
                }
                else
                {
                         //applies knockback to objects with gravity
                         if(hit.transform.GetComponent<Rigidbody>() != null)
                            hit.rigidbody.AddForce(-hit.normal * knockbackForce);

                    //***************************************************************************\\
                    //                                                                           \\
                    //                      Handles Damage for Explosions                        \\
                    //                                   \/                                      \\
                    //***************************************************************************\\

                    if (explosion != null)
                    {
                        float damage;
                        bool isCritical;

                        if (UnityEngine.Random.Range(0, 100) <= criticalStrikeChance)
                        {
                            damage = maxDamage * 2;
                            isCritical = true;
                        }
                        else
                        {
                            //I'm doing + 1 after maxDamage because Random.Range's never grab the max value.
                            damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                            isCritical = false;
                        }

                        var explosionDamage = Instantiate(explosion, hit.point, Quaternion.identity);
                        explosionDamage.GetComponent<Explosion>().AssignValues(minDamage, maxDamage, isCritical);
                        explosionDamage.GetComponent<Explosion>().damage = Mathf.RoundToInt(damage);
                    }



                    //***************************************************************************\\
                    //                                                                           \\
                    //                      This handles making bullet holes                     \\
                    //                                   \/                                      \\
                    //***************************************************************************\\

                    if (doBulletHoles & target == null)
                    {
                        GameObject t_bulletHole = bulletHole;
                        GameObject t_IncendiaryBulletHole = incendiaryBulletHole;


                        //------------------------ Creates normal bullet hole if their is no decal in the incendiary slot. ----------------------\\
                        if (incendiaryBulletHole != null & explosion != null)
                        {
                            //creating the bullet things
                            t_IncendiaryBulletHole = Instantiate(incendiaryBulletHole, hit.point + hit.normal * -0.5f, Quaternion.identity) as GameObject;

                            //makes bullet hole look in right direction
                            t_IncendiaryBulletHole.transform.LookAt(hit.point + hit.normal);

                            //attaches bullethole as parent to object for movement
                            t_IncendiaryBulletHole.transform.parent = hit.transform;

                            //destorys bullet after certain length
                            Destroy(t_IncendiaryBulletHole, bulletHoleLiveTime);
                        }
                        //------------------------ Creates incediary bullet hole if their is a gameobject in the slot. ----------------------\\
                        else
                        {
                            //creating the bullet things
                            t_bulletHole = Instantiate(bulletHole, hit.point + hit.normal * -0.5f, Quaternion.identity) as GameObject;

                            //makes bullet hole look in right direction
                            t_bulletHole.transform.LookAt(hit.point + hit.normal);

                            //attaches bullethole as parent to object for movement
                            t_bulletHole.transform.parent = hit.transform;

                            //destorys bullet after certain length
                            Destroy(t_bulletHole, bulletHoleLiveTime);
                        }
                    }
                }

                //***************************************************************************\\
                //                                                                           \\
                //                      This handles drawing bullet rays                     \\
                //                                   \/                                      \\
                //***************************************************************************\\
                if (drawBulletRay)
                {

                    GameObject bulletRayTracker = bulletRay;

                    //this creates ray
                    bulletRayTracker = Instantiate(bulletRay, lineRenderStartLocation.position, Quaternion.identity) as GameObject;

                    //this assignis the lin rendere value
                    LineRenderer line = bulletRayTracker.GetComponent<LineRenderer>();

                    //this sets line rendere locations
                    line.SetPosition(0, new Vector3(lineRenderStartLocation.position.x, lineRenderStartLocation.position.y, lineRenderStartLocation.position.z));
                    line.SetPosition(1, new Vector3(hit.point.x, hit.point.y, hit.point.z));

                    //this deletes ray
                    Destroy(bulletRayTracker, bulletRayLiveTime);
                }

            }

            if (drawBulletRay && !Physics.Raycast(fpsCam.GetComponent<Camera>().transform.position, t_bloom, out hit, range, ~cannotBeShot))
            {

                GameObject bulletRayTracker = bulletRay;

                //this creates ray
                bulletRayTracker = Instantiate(bulletRay, lineRenderStartLocation.position, Quaternion.identity) as GameObject;

                //this assignis the lin rendere value
                LineRenderer line = bulletRayTracker.GetComponent<LineRenderer>();

                //this deletes ray
                Destroy(bulletRayTracker, bulletRayLiveTime);
            }

        }

        //***************************************************************************\\
        //                                                                           \\
        //                      This handles recoil - Calls the camera               \\
        //                                   \/                                      \\
        //***************************************************************************\\

        if (transform.parent.GetComponent<Recoil>() != null)
        {
            //assigins gun variables
            transform.parent.GetComponent<Recoil>().recoilRotation = recoilRotation;
            transform.parent.GetComponent<Recoil>().rotationSpeed = rotationSpeed;
            transform.parent.GetComponent<Recoil>().returnSpeed = returnSpeed;

            //fires the recoil
            transform.parent.GetComponent<Recoil>().fire = true;
        }

        if(muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (shotDelaySound != null & !playedDelaySound)
        {
            StartCoroutine("PlaySoundDuringDelay");
            playedDelaySound = true;
        }

        //***************************************************************************\\
        //                                                                           \\
        //                      This handles the shooting sound                      \\
        //                                   \/                                      \\
        //***************************************************************************\\

        var audio = Instantiate(shootSound, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        audio.GetComponent<AudioSource>().volume = shotVolume;
        audio.GetComponent<AudioSource>().pitch = shotPitch;
        audio.transform.parent = GameObject.Find("AudioHolder").transform;
    }

    void BurstShoot()
    {
        isBursting = true;
        if(ammo >= burstAmount)
        {
            currentBurstAmount = burstAmount;
        }
        else
        {
            currentBurstAmount = ammo;
        }
    }

    void Reload()
    {
        if (ammo != maxAmmo && !isReloading && !isBursting && !isDelaying)
        {
            isReloading = true;
            reloadDuration = Time.time + reloadTime;

            //if reloadBulletsIndividual == true than the sound will be played at the end of the reload
            if (!reloadBulletsIndividual)
            {
                var audio = Instantiate(reloadSound, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                audio.transform.parent = GameObject.Find("AudioHolder").transform;
            }
        }
    }

    void UI()
    {
        float ammoFloat;
        float maxAmmoFloat;

        //changing intigers to floats for division
        ammoFloat = ammo;
        maxAmmoFloat = maxAmmo;

        if(!isReloading && !reloadBulletsIndividual)
        {
            gunIcon.fillAmount = ammoFloat / maxAmmoFloat;
            ammoUI.text = ammo + " / " + maxAmmo;
        }
        else if(reloadBulletsIndividual)
        {
            gunIcon.fillAmount = ammoFloat / maxAmmoFloat;
            ammoUI.text = ammo + " / " + maxAmmo;
        }
    }

    void ShootDelay()
    {
        isDelaying = true;
        currentShotDelay = Time.time + shotDelay;
    }

    void WeaponSway()
    {
        float movementX = -Input.GetAxis("Mouse X") * swayAmount;
        float movementY = -Input.GetAxis("Mouse Y") * swayAmount;

        movementX = Mathf.Clamp(movementX, -maxSwayAmount, maxSwayAmount);
        movementY = Mathf.Clamp(movementY, -maxSwayAmount, maxSwayAmount);


        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
            
        if(isAiming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + new Vector3(0, initialPosition.y, initialPosition.z), Time.deltaTime * smoothAmount);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
        }    
    }
    
    void GunAim()
    {

        aimingFov.aimFovChange = aimFovChange;
        aimingFov.aimFovSpeed = aimFovSpeed;

        if (Input.GetButton("Fire2") && playerMove.isGrounded)
        {
            if (!aimingFov.isAiming)
                aimingFov.isAiming = true;

            if (!isAiming)
                isAiming = true;

            if(bloom != initialBloom/2)
                bloom = initialBloom / 2;

            transform.localPosition = Vector3.Lerp(transform.localPosition,new Vector3(0,initialPosition.y,initialPosition.z), Time.deltaTime * smoothAmount);

            if(crosshair != null && !crosshair.GetComponent<Reticle>().lookingAtItem)
            {
                crosshair.GetComponent<Reticle>().aiming = true;
                crosshair.GetComponent<Reticle>().waitTime = Time.time + 0.1f;
            }
        }
        else
        {
            if(aimingFov.isAiming)
                aimingFov.isAiming = false;

            if (isAiming)
                isAiming = false;

            if(bloom != initialBloom)
                bloom = initialBloom;

            if (crosshair != null)
            {
                crosshair.GetComponent<Reticle>().aiming = false;
            }
        }
    }

    IEnumerator PlaySoundDuringDelay()
    {
        yield return new WaitForSeconds(timeUntilDelaySoundPlayed);

        var audio = Instantiate(shotDelaySound, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        audio.transform.parent = GameObject.Find("AudioHolder").transform;
    }
}
