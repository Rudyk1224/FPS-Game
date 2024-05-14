using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponAim //enumerations are used to assign constant names to a group of integers
{
    NONE, //no aim (i.e: axe)
    SELF_AIM, //self aim (i.e: bow, spear)
    AIM //aim (i.e: shotgun, assault rifle, revolver)
}

public enum WeaponFireType
{
    SINGLE, //(i.e: revolver,shotgun,single bullet)
    MULTIPLE //(i.e: assault rifle fires multiple bullets)
}

public enum WeaponBulletType
{
    BULLET,
    ARROW,
    SPEAR,
    NONE
}

public class WeaponHandler : MonoBehaviour
{
    private Animator anim;

    public WeaponAim weapon_Aim;
    [SerializeField]
    private GameObject muzzleFlash;
    [SerializeField]
    private AudioSource shootSound, reloadSound;

    public WeaponFireType fireType;
    public WeaponBulletType bulletType;
    public GameObject attack_Point;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void ShootAnimation()
    {
        anim.SetTrigger(AnimationTags.SHOOT_TRIGGER); //this will set the trigger for shoot inside the animator tab 
    }
    public void Aim(bool canAim)
    {
        anim.SetBool(AnimationTags.AIM_PARAMETER, canAim); //canAim will be true when we aim, and false when we do not aim
    }
    void Turn_On_Muzzle_Flash()
    {
        muzzleFlash.SetActive(true);
    }
    void Turn_Off_Muzzle_Flash()
    {
        muzzleFlash.SetActive(false);
    }
    void Play_ShootSound()
    {
        shootSound.Play();
    }
    void Play_ReloadSound()
    {
        reloadSound.Play();
    }
    void Turn_On_AttackPoint()
    {
        attack_Point.SetActive(true);
    }
    void Turn_Off_AttackPoint()
    {
        if (attack_Point.activeInHierarchy)
            attack_Point.SetActive(false);
    }
}
