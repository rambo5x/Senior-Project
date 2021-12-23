using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class DeprecatedWeapon : Item
{
  public GameObject prefab;
  public int magSize;
  public int magCount;
  public float range;
  public WeaponClass weaponClass;
}
public enum WeaponClass 
{ Primary, 
Secondary, 
Melee
}
