using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { GUN, MELEE, THROWABLE }
public class Weapon : MonoBehaviour
{
    public GameObject icon;
    public WeaponType weaponType;
}
