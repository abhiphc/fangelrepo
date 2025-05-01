using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WeaponSelector : MonoBehaviour
{
    [SerializeField] bool isSelected = false;
    public int selectedWeaponIndex;
    [SerializeField] List<Transform> weapons = new List<Transform>();
    void Start()
    {
        selectedWeaponIndex = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) // Gun mode can activate or deactivate through this key
        {
            if(!Input.GetMouseButton(1)) // While pressing RMB the gun mode cannot be activated
            {
                ToggleGunMode();
            }
            
        }
       
    }
    void ToggleGunMode()
    {
            isSelected = !isSelected;

                if (isSelected)
                {
                    PlayerGameMech.hasGun = true;
                    SelectGun();    
        }
                else
                {
                    PlayerGameMech.hasGun = false;
                }
    }

    void SelectGun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeaponIndex = 0;
            weapons[0].gameObject.SetActive(true);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeaponIndex = 1;
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(true);
            weapons[2].gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeaponIndex = 2;
            weapons[0].gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(true);
        }
        else
        {
            selectedWeaponIndex = 0;
            weapons[0].gameObject.SetActive(true);
            weapons[1].gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(false);
        }
    }
}
