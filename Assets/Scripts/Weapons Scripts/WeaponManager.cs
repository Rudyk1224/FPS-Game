using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour //this class allows it to select different weapons
{
    [SerializeField]
    private WeaponHandler[] weapons; //array of weapons (axe,revolver,shotgun,assault rifle,spear,bow)

    private int currentWeaponIndex;
    // Start is called before the first frame update
    void Start()
    {
        currentWeaponIndex = 0;
        weapons[currentWeaponIndex].gameObject.SetActive(true); //turns on the 0th element in the weapons gameObject array 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //if user presses 1
        {
            TurnOnSelectedWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) //if user presses 2
        {
            TurnOnSelectedWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) //if user presses 3
        {
            TurnOnSelectedWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) //if user presses 4
        {
            TurnOnSelectedWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) //if user presses 5
        {
            TurnOnSelectedWeapon(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) //if user presses 6
        {
            TurnOnSelectedWeapon(5);
        }
    }
    void TurnOnSelectedWeapon(int weaponIndex)
    {
        if (weaponIndex == currentWeaponIndex) //avoids the same weapon being drawn twice
            return;
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        weapons[weaponIndex].gameObject.SetActive(true);
        currentWeaponIndex = weaponIndex;
    }
    public WeaponHandler GetCurrentSelectedWeapon() //returns what weapon we are currently using (to later acess its specific info like aim and bullet type)
    {
        return weapons[currentWeaponIndex];
    }

}
