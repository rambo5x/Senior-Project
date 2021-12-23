using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text ammo;
    private List<GameObject> weaponIndicatorPrefabs = new List<GameObject>();
    private List<GameObject> weaponIndicatorObjects = new List<GameObject>();
    public const int elementOffset = -30;
    public const int uiStartOffset = 100;
    public GameObject uiWeaponsHolder;
    [System.NonSerialized] public int previouslyDisplayedWeapon = -1, previouslyRemovedWeapon = -1; // sentinel value
    [System.NonSerialized] public GameObject weaponsHolder;
    [System.NonSerialized] private static UIManager _instance;

    public static UIManager Instance { get {return _instance; } }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void SetAmmo(string ammoCount)
    {
        ammo.text = ammoCount;
    }

    public void SetWeaponToDisplay(int index)
    {
        for (int i = 0; i < weaponIndicatorObjects.Count; i++){
            if (i == index){
                weaponIndicatorObjects[i].SetActive(true);
                previouslyDisplayedWeapon = i;
                switch(weaponIndicatorPrefabs[i].GetComponent<Weapon>().weaponType){
                    case WeaponType.MELEE:
                        SetAmmo("");
                        break;
                    case WeaponType.THROWABLE:
                        SetAmmo("1");
                        break;
                }
            } else {
                weaponIndicatorObjects[i].SetActive(false);
            }
        }
        
    }

    public void AddWeapon(GameObject newWeapon){
        foreach(GameObject weapon in weaponIndicatorPrefabs){
            if (weapon == newWeapon) return;
        }
        weaponIndicatorPrefabs.Add(newWeapon);
        DrawUI();
    }

    public void InitializeWeapons(GameObject weaponsHolder){
        this.weaponsHolder = weaponsHolder;
        for (int i = 0; i < weaponsHolder.transform.childCount; i++){
            weaponIndicatorPrefabs.Add(weaponsHolder.transform.GetChild(i).gameObject);
        }
        DrawUI();
        for (int i = 0; i < weaponIndicatorPrefabs.Count; i++){
            weaponIndicatorObjects[i].SetActive(false);
        }
    }

    public void RemoveWeapon(GameObject removeWeapon){
        for (int i = 0; i < weaponIndicatorPrefabs.Count; i++){
            if (removeWeapon == weaponIndicatorPrefabs[i]) {
                weaponIndicatorPrefabs.RemoveAt(i);
                previouslyRemovedWeapon = i;
                break;
            }
        }
        DrawUI();
    }

    public void DrawUI(){
        for (int i = 0; i < uiWeaponsHolder.transform.childCount; i++){
            Destroy(uiWeaponsHolder.transform.GetChild(i).gameObject);
        }
        weaponIndicatorObjects = new List<GameObject>();

        int position = uiStartOffset;
        foreach (GameObject weapon in weaponIndicatorPrefabs){
            weaponIndicatorObjects.Add(Instantiate(weapon.GetComponent<Weapon>().icon, new Vector3(uiWeaponsHolder.transform.position.x, uiWeaponsHolder.transform.position.y + position, uiWeaponsHolder.transform.position.z), Quaternion.identity, uiWeaponsHolder.transform));
            position += elementOffset;
        }

        if (previouslyRemovedWeapon != -1 && previouslyDisplayedWeapon == previouslyRemovedWeapon) {
            weaponsHolder.GetComponent<SwitchWeapon>().SelectWeapon(previouslyDisplayedWeapon == 0 ? weaponIndicatorObjects.Count - 1 : previouslyDisplayedWeapon - 1);
        } else if (previouslyDisplayedWeapon != -1){
            weaponsHolder.GetComponent<SwitchWeapon>().SelectWeapon(previouslyDisplayedWeapon);
        }
    }
}
