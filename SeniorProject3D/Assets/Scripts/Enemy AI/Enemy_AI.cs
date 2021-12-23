using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    public GameObject Player;
    public float Distance;
    public NavMeshAgent _agent;
    Animator enemy;
    int atk;
    

    void Start()
    {
        enemy = GetComponent<Animator>();
        Target target = GetComponent<Target>();
        // just for better performance
        atk = Animator.StringToHash("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        bool inAtkRange = enemy.GetBool(atk);
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);
        _agent.SetDestination(Player.transform.position);

        // if in atk distance do atk animation, not then switch to walking animation
        if(Distance <= 2.5f && !inAtkRange)
        {
            enemy.SetBool("Attack", true);
        }
        if(Distance > 2.5f && inAtkRange)
        {
            enemy.SetBool("Attack", false);
        }
    }

}
