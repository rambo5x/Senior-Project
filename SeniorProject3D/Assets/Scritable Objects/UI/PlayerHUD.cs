using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] public WeaponUI weaponUI;
    public void UpdateWeaponUI(Weapon newWeapon)
    {
        // weaponUI.UpdateInfo(newWeapon.Icon, newWeapon.magSize, newWeapon.magCount);
    }
}

