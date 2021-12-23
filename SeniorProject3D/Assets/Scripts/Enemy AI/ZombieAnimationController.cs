using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationController : MonoBehaviour
{
    Animator enemy;
    int isWalkinghash;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Animator>();

        // just for better performance
        isWalkinghash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {   
        // Gets the bool value from the tag editor
        bool isWalking = enemy.GetBool(isWalkinghash);
        bool key_press = Input.GetKey("w");

        if(!isWalking && key_press)
        {
            enemy.SetBool("isWalking", true);
        }

        if(isWalking && !key_press)
        {
            enemy.SetBool("isWalking", false);
        }
    }
}
