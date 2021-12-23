using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByeBye : MonoBehaviour
{

    void OnTriggerEnter (Collider touch)
    {
        if(touch.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
    
}
