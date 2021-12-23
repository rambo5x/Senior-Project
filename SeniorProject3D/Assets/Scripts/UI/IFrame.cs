using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IFrame : MonoBehaviour
{

    public HP_Bar_Test HealthTrack; 
    public Image fill;

    Color immunity; // immunity
    Color normal; // normla red

    // Start is called before the first frame update
    void Start()
    {
        normal = this.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {   
        if(!HealthTrack.takedmg)
        {
            fill.color = new Color32(68,55,55,255);
        }
        else
        {
            fill.color = normal;
        }
    }

    /*
    void Immune()
    {
        timer -= Time.deltaTime;
        fill.color = new Color32(68,55,55,255);
        Debug.Log("Immunity Granted");
    }

    void refresh()
    {
        timer = 2f;
        fill.color = normal;
        Debug.Log("Immunity Off");
    }
    */
}

