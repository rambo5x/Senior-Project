using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tele_Enemy : MonoBehaviour
{
    public GameObject Player;
    public NavMeshAgent _agent;
    public float Distance;
    Animator enemy;
    int atk;

    // Teleport Location Trackers
    float enemyX;
    float enemyZ;
    float newX;
    float newZ;

    //?
    Vector3 pos;
    public float speed = 1f;

    // health tracker
    float current_health;

    // Timer
    public float timeRemaining = 6;
    public bool timerIsRunning = false;
    

    void Start()
    {
        enemy = GetComponent<Animator>();
        Target target = GetComponent<Target>();

        // just for better performance
        atk = Animator.StringToHash("Attack");
        current_health = target.health;
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        Target target = GetComponent<Target>();
        bool inAtkRange = enemy.GetBool(atk);
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);

        // wait timer if no hit
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                enemy.SetBool("Run", true);
                _agent.SetDestination(Player.transform.position);
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
        else
        {
            _agent.SetDestination(Player.transform.position);
        }


        // enemy takes dmg, Teleport and start chasing
        if(current_health > target.health)
        {   
            enemy.SetBool("Run", true);
            _agent.SetDestination(Player.transform.position);
            current_health = target.health;
            timeRemaining = 0;
            timerIsRunning = false;
            // find which Z position is bigger
            if(Player.transform.position.z >= this.transform.position.z)
            {
                newZ = (Player.transform.position.z - this.transform.position.z) * 2;
                if(Distance < 2f)
                {
                    newZ += this.transform.position.z + 5;
                }
                newZ += this.transform.position.z;
                newX = (Player.transform.position.x - this.transform.position.x) * 2;
                newX += this.transform.position.x;
            }
            if(Player.transform.position.z < this.transform.position.z)
            {
                newZ = (Player.transform.position.z - this.transform.position.z) * 2;
                if(Distance < 2f)
                {
                    newZ += this.transform.position.z + 5;
                }
                newZ += this.transform.position.z;
                newX = (Player.transform.position.x - this.transform.position.x) * 2;
                newX += this.transform.position.x;
            }
            // (X,Y,Z)
            this.transform.position = new Vector3(newX, 0.5f, newZ);
            Rotate();
        }
        
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
    
    void Rotate () {
        enemy.transform.LookAt(Player.transform);
    }
}
