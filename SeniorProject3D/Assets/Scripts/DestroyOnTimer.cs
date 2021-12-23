using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTimer : MonoBehaviour
{
    public float time;

    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, time);
    }
}
