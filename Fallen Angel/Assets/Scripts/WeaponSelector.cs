using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] bool isSelected = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Gun mode can activate or deactivate through this key
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
                }
                else
                {
                    PlayerGameMech.hasGun = false;
                }
    }
}
