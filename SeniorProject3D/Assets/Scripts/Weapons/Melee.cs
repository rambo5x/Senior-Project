using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    [SerializeField] public Animator animator;
    [SerializeField] public float meleeDmg = 50f;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetBool("attacking", true);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            animator.SetBool("attacking", false);
        }
    }

    void meleeAttack()
    {
        animator.SetBool("attacking", true);
    }

    void OnTriggerEnter(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if(target != null)
        {
            Debug.Log("hit");
            target.TakeDamage(meleeDmg);
        }
    }
}
