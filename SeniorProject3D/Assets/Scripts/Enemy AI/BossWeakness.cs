using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossWeakness : MonoBehaviour
{

    public Material Current_Weakness, AR, Pistol, Sniper, Grenades;
    public GameObject WeaponHolder;
    public float Weakness_Switch;

    Animator enemy;
    CapsuleCollider e_Collider;
    float maxTime;
    int weak;
    int prev_weak;
    Material[] meshMat;

    // Start is called before the first frame update
    void Start()
    {
        prev_weak = 2;
        weak = 2;
        maxTime = Weakness_Switch;
        meshMat = gameObject.GetComponent<Renderer>().materials;
        e_Collider = GetComponentInParent<CapsuleCollider>();
        enemy = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {       
        
        // immunity from weapon if not correct
        if(!WeaponHolder.transform.GetChild(weak - 1).gameObject.activeInHierarchy)
        {
            //Debug.Log("Blocking");
            e_Collider.enabled = false;
            enemy.SetBool("Block", true);
        }
        else
        {
            //Debug.Log("Weakness");
            e_Collider.enabled = true;
            enemy.SetBool("Block", false);
        }
        
        // color change & weakness change
        if (Weakness_Switch > 0)
        {
            Weakness_Switch -= Time.deltaTime;
        }
        else
        {
            PlayChange();
            newWeakness();
            Weakness_Switch = maxTime;
        }
    }


    //have time to play animation
    void PlayChange()
    {
        enemy.SetTrigger("WeakChange");
    }

    void newWeakness()
    {   bool newWeak = true;
        while(newWeak)
        {
            weak = Random.Range(1,4); // change to (1,5) when adding grenades
            if(weak != prev_weak)
            {
                newWeak = false;
                prev_weak = weak;
            }
        }
        //Debug.Log(weak);

        if(weak == 1)
        {
            meshMat[1] = AR;
        }
        else if(weak == 2)
        {
            meshMat[1] = Pistol;
        }
        else if(weak == 3)
        {
            meshMat[1] = Sniper;
        }
        else
        {
            meshMat[1] = Grenades;
        }
        this.GetComponent<Renderer>().materials = meshMat;
    }
}
