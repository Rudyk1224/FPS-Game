using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private WeaponManager weapon_Manager;
    public float fireRate = 15f; //for assault rifle only. We will fire every 0.15 of a second
    private float nextTimeToFire;
    public float damage = 20f;

    private Animator zoomCameraAnim;
    private bool Zoomed;

    private Camera mainCam;
    private GameObject crosshair;
    private bool is_Aiming;

    [SerializeField]
    private GameObject arrowPrefab, spearPrefab;
    [SerializeField]
    private Transform arrow_Bow_StartPosition;

    void Awake()
    {
        weapon_Manager = GetComponent<WeaponManager>();
        //1st transform --> transform of the player. Transform.find() finds the child with the given tag and finds its transform (lookroot)
        //2nd transform --> finds the child of Look Root and finds its transform (zoom camera)
        //Finally, get the animator component from the child's child (zoom camera/fp camera)
        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT).transform.Find(Tags.ZOOM_CAMERA ).GetComponent<Animator>();

        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR); //finds and returns the gameObject with a tag (creates reference for crosshair)

        mainCam = Camera.main; //gets access to main camera
    }
 

    // Update is called once per frame
    void Update()
    {
        WeaponShoot();
        ZoomInAndOut();
    
    }
    void WeaponShoot()
    {
        if (weapon_Manager.GetCurrentSelectedWeapon().fireType == WeaponFireType.MULTIPLE) //if we have an assault rifle. We first get acess to the weapon manager script and its method. Then, since the weaponmanager script has an array that contains all the weapons, and since those weapons have the WeaponHandler script, we get acess to all its public variables and methods too
        {
            //if we press and HOLD the left mouse button click (0=leftside, 1=rightside), 
            //AND if time is greater than the nextTimeToFire
            if (Input.GetKey(KeyCode.E) && Time.time > nextTimeToFire) //time.time is how many seconds has passed since we run our game. MouseButton = press and hold. MouseButtonDown = press down once
            {
                nextTimeToFire = Time.time + 1f / fireRate;
      
                weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                BulletFired();
            }

        }
        else //if we don't have an assault rifle (regular weapon that shoots once)
        {
          
            if (Input.GetKeyDown(KeyCode.E)){ //if we press the left mouse botton
                
                if (weapon_Manager.GetCurrentSelectedWeapon().tag == Tags.AXE_TAG) //if the current weapon is the axe
                {
                    weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                }

                if(weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.BULLET) //if the current weapon is a weapon that shoots bullets
                {
                    weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                    BulletFired();
                }
                else //we have an arrow or spear (no bullets)
                {
                    if (is_Aiming)
                    {
                        weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation(); //shoot the arrow or spear (by turning on a trigger). We go from aiming to shooting

                        if(weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.ARROW)
                        {
                            //throw arrow
                            ThrowArrowOrSpear(true);

                        } else if(weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.SPEAR)
                        {
                            //throw spear
                            ThrowArrowOrSpear(false);
                        }
                    }
                }
            }
           
        }
    }

    void ZoomInAndOut()
    {
        //we are going to aim with the animation we created
        if (weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.AIM) //get access to the current weapon and it's aim type, and see if it matches the AIM property of the weapon aim enum
        {
            //if we press and hold right mouse button
            if (Input.GetMouseButtonDown(1))
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM); //play the zoom in animation
                crosshair.SetActive(false); //turn off crosshair
            }

            //when we release the right mouse button
            if (Input.GetMouseButtonUp(1))
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM); //play the zoom out animation
                crosshair.SetActive(true); //turn back on crosshair
            }
        }


        if (weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.SELF_AIM) //if its a self aim weapon
        {
            if (Input.GetMouseButton(1))
            {
            weapon_Manager.GetCurrentSelectedWeapon().Aim(true); //set the aim function to be true in weapon handler which will play the animation
            is_Aiming = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                weapon_Manager.GetCurrentSelectedWeapon().Aim(false); //set the aim function to be false in weapon handler which will not play the animation
                is_Aiming = false;
            }
        }
    }

    void ThrowArrowOrSpear(bool throwArray)
    {
        if (throwArray) //if its an arrow
        {
            GameObject arrow = Instantiate(arrowPrefab); //instantiate creates a copy of our prefab
            arrow.transform.position = arrow_Bow_StartPosition.position; //set arrow's position to the starting position
            arrow.GetComponent<ArrowBowScript>().Launch(mainCam); //gets reference to the arrowbowscript and calls its launch function

        }
        else //if its a spear
        {
            GameObject spear = Instantiate(spearPrefab); 
            spear.transform.position = arrow_Bow_StartPosition.position; 
            spear.GetComponent<ArrowBowScript>().Launch(mainCam);
            //Important side note: If you drag and drop a gameObject (i.e: starting position) on another gameObject (i.e: Player), and you change the name of that gameObject inside the script, the gameObject that you drag and dropped will be gone and you will have to redrag it

        }
    }
    void BulletFired()
    {
        RaycastHit hit;
        //we will raycast from the origin (the camera's original position) towards the intended direction (the camera's forward position), which will be raycasted infinately
        //hit contains the information of the gameObject that we hit. The out keyword causes arguments to be passed by reference (so we have references that point to hit)
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit)) 
        {
            //so now we can get anything from hit, which is the gameObject that we hit, including its different components (i.e: hit.transform.gameObject.GetComponent<something>() )
           if(hit.transform.tag == Tags.ENEMY_TAG)
            {

                if (weapon_Manager.GetCurrentSelectedWeapon().fireType == WeaponFireType.MULTIPLE)
                {
                    damage = GetComponent<HealthScript>().health / 4;
                }
                hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }
    }
}
