using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public NavMeshAgent _agent;
    Animator enemy;
    CapsuleCollider e_Collider;

    void Start()
    {
        enemy = GetComponent<Animator>();
        e_Collider = this.GetComponent<CapsuleCollider>();
    }
   public void TakeDamage(float amount)
    {
        health -= amount;
        
        if(health <= 0f)
        {   
            //stop chase and play dead
            Die();
        }   
    }

    public void TakeDamageImmobile(float amount)
    {
        health -= amount;
        
        if(health <= 0f)
        {   
            //stop chase and play dead
            DieImmobile();
        }
    }

    void Die()
    { 
        e_Collider.enabled = false;
        e_Collider.isTrigger = false;
        _agent.isStopped = true; 
        _agent.velocity = Vector3.zero;
        enemy.SetBool("Dead", true);
        if (gameObject.tag == "Final Boss") SceneManager.LoadScene(2);
        Destroy(gameObject, 6);
    }

    void DieImmobile()
    { 
        Destroy(gameObject);
    }
}
