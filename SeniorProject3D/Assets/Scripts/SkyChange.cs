using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyChange : MonoBehaviour
{
    public Material mat1;
    public Material mat2;
    public bool bloodphase;
    public DayNightCycle cycle;
    // Timer
    public float maxTime;
    public float timeRemaining;

    // Start is called before the first frame update
    void Awake()
    {
        RenderSettings.skybox = mat1;
        bloodphase = true;
        timeRemaining = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        // BloodPhase when Final boss is out
        if(ChunkController.Instance.boss != null)
        {
            RenderSettings.skybox = mat2;
            bloodphase = false;
        }
        else
        {
            //remain in current skybox
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0 && !cycle.dayTime)
            {   
                // change sky and run time again
                Change();
                timeRemaining = maxTime;
            }
        }
    }

    void Change()
    {
        if(bloodphase == true)
        {
            RenderSettings.skybox = mat2;
            bloodphase = false;
            ChunkController.Instance.SpawnBoss();
            ChunkController.Instance.isBloodMoon = true;
        }
        else
        {
            RenderSettings.skybox = mat1;
            bloodphase = true;
        } 
    }
}
