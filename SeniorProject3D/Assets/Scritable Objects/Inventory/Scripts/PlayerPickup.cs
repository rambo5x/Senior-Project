using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
   [SerializeField] private float pickupRange;
   [SerializeField] private LayerMask pickupLayer;

   private Camera cam;
   private Inventory inventory;
   private void Start()
   {
       GetReferences();
   }
   private void Update()
   {
       if(Input.GetKeyDown(KeyCode.E))
       {
           Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
           RaycastHit hit;
           if(Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
           {
               Debug.Log("Hit: " + hit.transform.name);
            //    Weapon newWeapon = hit.transform.GetComponent<ItemObject>().item as Weapon;
            //    inventory.AddItem(newWeapon);
               Destroy(hit.transform.gameObject);
           }
       }
   }
   // Testing via Collision \/
   /*
   void OnTriggerEnter(Collider col)
   {
       if(col.gameObject.tag == "Weapon")
       {
           Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
           RaycastHit hit;
           if(Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
           {
               Debug.Log("Hit: " + hit.transform.name);
               Weapon newWeapon = hit.transform.GetComponent<ItemObject>().item as Weapon;
               inventory.AddItem(newWeapon);
               Destroy(hit.transform.gameObject);
           }
       }
   }
   */
  /*  void OnTriggerEnter(Collider col)
   {
       if(col.gameObject.tag == "Weapon")
       {
           Destroy(gameObject);
       }
   }
*/
   private void GetReferences()
   {
       cam = GetComponentInChildren<Camera>();
       inventory = GetComponent<Inventory>();
   }
}
