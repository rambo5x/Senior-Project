using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Explosion : MonoBehaviour
{
    public GameObject explosion, Player;
    public float Distance;
    public NavMeshAgent _agent;
    public float range;
    CapsuleCollider e_Collider;
    Animator enemy;
    public float Timer;

    void Awake()
    {
        explosion.SetActive(false);
    }

    void Start()
    {
        enemy = GetComponent<Animator>();
        e_Collider = this.GetComponent<CapsuleCollider>();
        Target target = GetComponent<Target>();
    }

    void Update()
    {
        
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);
        _agent.SetDestination(Player.transform.position);

        if(Distance <= range - 1)
        {
            enemy.SetBool("Explode", true);
            Explode();
            
        }
    }

    public void Explode()
    {
        e_Collider.radius += range;
        explosion.SetActive(true); 

        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            Timer = 0;
            e_Collider.enabled = false;
        }
        _agent.isStopped = true; 
        _agent.velocity = Vector3.zero;
        Destroy(gameObject, 3);

    }

   
}
