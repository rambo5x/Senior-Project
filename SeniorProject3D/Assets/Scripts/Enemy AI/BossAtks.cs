using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAtks : MonoBehaviour
{
    public LookAt look;
    public GameObject Player, CloseRangeAtk, LongRangeAtk;
    public CapsuleCollider CR_Collider;
    public BoxCollider LR_Collider;
    public NavMeshAgent _agent;
    public float Distance;
    public float AtkTime, AtkTimeSave;

    Animator enemy;

    void awake()
    {
        CloseRangeAtk.SetActive(false);
        LongRangeAtk.SetActive(false);
        CR_Collider.enabled = false;
        LR_Collider.enabled = false;
        AtkTimeSave = AtkTime;
    }
    // Start is called before the first frame update
    void Start()
    {
       enemy = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);

        //AtkTimer
        if (AtkTime > 0)
        {
            AtkTime -= Time.deltaTime;
            idle();
        }
        else //Atk
        {
            look.enabled = false;
            if(Distance <= 8f) // close range
                PlayCloseExp();
            else if(Distance >= 25f) // long range
                PlayLongExp();
            else                    // fast dash (mid range)
                SprintAtk();
            AtkTime = AtkTimeSave;
        }
      
    }

    void idle()
    {
        enemy.SetBool("Close Exp",false);
        enemy.SetBool("Long Exp",false);
        enemy.SetBool("SprintAtk",false);
        

        //_agent.isStopped = true; 
        //_agent.velocity = Vector3.zero;

        
    }

    void SprintAtk()
    {
        float sprintTime = 1f;
        enemy.SetBool("SprintAtk",true);

        while(sprintTime > 0)
        {  
            _agent.isStopped = false;
            _agent.SetDestination(Player.transform.position);
            sprintTime -= Time.deltaTime;
        }
        //_agent.isStopped = true; 
        //_agent.velocity = Vector3.zero;
    }
    
    void PlayLongExp()
    {
        look.enabled = false;
        enemy.SetBool("Long Exp",true);
        StartCoroutine (delayLRatk());
    }
    void PlayCloseExp()
    {
        look.enabled = true;
        enemy.SetBool("Close Exp",true);
        StartCoroutine (delayCRatk());
    }
    
    // Long Range Attack
    IEnumerator delayLRatk()
    {
        yield return new WaitForSeconds (0.90f);
        LongRangeAtk.SetActive(true);
        LR_Collider.enabled = true;
        StartCoroutine (delayResetLRatk());
    }

    IEnumerator delayResetLRatk()
    {
        yield return new WaitForSeconds (0.90f);
        LongRangeAtk.SetActive(false);
        LR_Collider.enabled = false;
        look.enabled = true;
    }


    // Close Range Atk
    IEnumerator delayCRatk()
    {
        yield return new WaitForSeconds (0.90f);
        CloseRangeAtk.SetActive(true);
        CR_Collider.enabled = true;
        StartCoroutine (delayResetCRatk());
    }

    
    IEnumerator delayResetCRatk()
    {
        yield return new WaitForSeconds (0.90f);
        CloseRangeAtk.SetActive(false);
        CR_Collider.enabled = false;
    }
    
}
