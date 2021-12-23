using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
 [SerializeField] private Weapon[] weapons;
 private PlayerHUD hud;
 //public Weapon testPrimary;
 //public Weapon testSecondary;
 private void Start()
 {
    GetReferences();
    InitVariables();
 }
/* private void Test()
 {
     if(Input.GetKeyDown(KeyCode.U))
     {
        AddItem(testPrimary);
     }
     if(Input.GetKeyDown(KeyCode.I))
     {
        AddItem(testSecondary);
     }
 }
 */
 public void AddItem(Weapon newItem)
 {
    //  int newItemIndex = (int)newItem.weaponClass;
    //  if(weapons[newItemIndex] != null)
    //  {
    //     RemoveItem(newItemIndex);
    //  }
    //     weapons[newItemIndex] = newItem;

    // // update weapon slot ui
    // hud.UpdateWeaponUI(newItem);

 }
 public void RemoveItem(int index)
 {
     weapons[index] = null;
 }
 public Weapon GetItem(int index)
 {
     return weapons[index];
 }
 private void InitVariables()
 {
     // Primary = 0 / Secondary = 1 / Melee = 2
     weapons = new Weapon[3];
 }
 private void GetReferences()
 {
     hud = GetComponent<PlayerHUD>();
 }
}
