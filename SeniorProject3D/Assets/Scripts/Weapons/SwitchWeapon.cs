using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{
    public int selectedWeapon = 0;
    public bool weaponSwitchingEnabled = true;
    public PlayerHUD hud;
    public int previousWeaponCount = 0;
    void Start()
    {
       UIManager.Instance.InitializeWeapons(gameObject);
       SelectWeapon(0);
    }

    void Update()
    {
        int previousWeaponSelected = selectedWeapon;
        if (weaponSwitchingEnabled){
            if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if(selectedWeapon >= transform.childCount - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon++;
                }
            }

            if(Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if(selectedWeapon <= 0)
                {
                    selectedWeapon = transform.childCount - 1;
                }
                else
                {
                    selectedWeapon--;
                }
            }

            /*Map guns to alpha keys if needed aside from scroll wheel*/
            if(Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 0;
            if(Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2) selectedWeapon = 1;
            if(Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3) selectedWeapon = 2;
        }

        if(previousWeaponSelected != selectedWeapon)
        {
            SelectWeapon(selectedWeapon);
        }
    }

    public void SelectWeapon(int selectWeapon)
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {
            if(i == selectWeapon)
            {
                weapon.gameObject.SetActive(true);
                UIManager.Instance.SetWeaponToDisplay(i);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

     private void GetReferences()
    {
        hud = GetComponent<PlayerHUD>();
    }
}
