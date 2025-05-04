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
                ShowGunsImage();
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
            ShowGunsImage();
        }
    }

    void DisplayGuns()
    {
        selectedWeaponIndex++;
        if (selectedWeaponIndex > weaponCount - 1)
        {
            selectedWeaponIndex = 0;
        }
        weaponIndex = selectedWeaponIndex;
        
    }
    void ShowGunsImage()
    {
        switch(selectedWeaponIndex)
        {
            case 0:
                Color tmpColor = weaponImages[0].color;
                tmpColor.a = 1f;
                weaponImages[0].color = tmpColor;
                for (int i=1; i<=weaponCount-1;i++)
                {
                    Color tmpColor1 = weaponImages[i].color;
                    tmpColor1.a = 0.2f;
                    weaponImages[i].color = tmpColor1;
                }
                break;
            case 1:
                Color tmpColor2 = weaponImages[1].color;
                tmpColor2.a = 1f;
                weaponImages[1].color = tmpColor2;

                for (int i = 0; i <= weaponCount-1; i++)
                {
                    if( i == 1)
                    {
                        continue;
                    }
                    Color tmpColor3 = weaponImages[i].color;
                    tmpColor3.a = 0.2f;
                    weaponImages[i].color = tmpColor3;
                }
                break;
            case 2:
                Color tmpColor4 = weaponImages[2].color;
                tmpColor4.a = 1f;
                weaponImages[2].color = tmpColor4;

                for (int i = 0; i <= weaponCount-1; i++)
                {
                    if(i == 2)
                    {
                        continue;
                    }
                    Color tmpColor5 = weaponImages[i].color;
                    tmpColor5.a = 0.2f;
                    weaponImages[i].color = tmpColor5;
                }
                break;
            case 3:
                Color tmpColor6 = weaponImages[3].color;
                tmpColor6.a = 1f;
                weaponImages[3].color = tmpColor6;
                for (int i = 0; i < weaponCount-1; i++)
                {
                    Color tmpColor7 = weaponImages[i].color;
                    tmpColor7.a = 0.2f;
                    weaponImages[i].color = tmpColor7;
                }
                break;

        }
    }
}
