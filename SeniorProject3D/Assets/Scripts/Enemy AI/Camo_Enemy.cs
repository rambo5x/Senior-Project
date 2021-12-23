using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Camo_Enemy : MonoBehaviour
{
    public GameObject Player;
    public NavMeshAgent _agent;
    public float Distance;
    Animator enemy;
    int atk;
    
    // Start is called before the first frame update
    void Start()
    {
       enemy = GetComponent<Animator>();
       Target target = GetComponent<Target>();
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
