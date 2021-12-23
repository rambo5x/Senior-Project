using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinding : MonoBehaviour
{
    public GameObject Blindbox;
    float Timer;
    bool Blinded;

    void Awake()
    {
        Blindbox.SetActive(false);
        Timer = 0;
        Blinded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Blinded)
        {
            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                Timer = 0;
                Blinded = false;
                Blindbox.SetActive(false);
            }
        }
    }

    void OnTriggerEnter (Collider touch)
    {
         if(touch.gameObject.tag == "Shadow")
        {
            Blindbox.SetActive(true);
            Timer = 3f;
            Blinded = true;
            Debug.Log("Blinded");
        }
    }
}
