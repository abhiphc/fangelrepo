using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class WeaponSelector : MonoBehaviour
{
    [SerializeField] bool isSelected = false;
    public static int selectedWeaponIndex =0; //this variable is used to select the weapon in the PlayerGameMech script
    [SerializeField] int weaponIndex; //for seeing in inspector
    [SerializeField] int weaponCount = 4;
    [SerializeField] List<Image> weaponImages = new List<Image>();
    private void Start()
    {
        weaponIndex = selectedWeaponIndex;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) // Gun mode can be activated or deactivated through this key
        {
            if(!Input.GetMouseButton(1)) // While pressing RMB the gun mode cannot be activated
            {
                ToggleGunMode();
            }
            
        }
        if (isSelected) 
        {
            SelectGun();
        }

    }
    void ToggleGunMode()
    {
            isSelected = !isSelected;

                if (isSelected)
                {
                    PlayerGameMech.hasGun = true;
                       
        }
                else
                {
                    PlayerGameMech.hasGun = false;
                }
    }

    void SelectGun()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayGuns();
        }
    }

    void DisplayGuns()
    {
        Color tmpColor = weaponImages[weaponIndex].color;
        tmpColor.a = 1f;
        weaponImages[weaponIndex].color = tmpColor;
        selectedWeaponIndex++;

        if (selectedWeaponIndex > weaponCount - 1)
        {
            selectedWeaponIndex = 0;
        }
        weaponIndex = selectedWeaponIndex;
    }
}
