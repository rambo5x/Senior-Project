using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;
    public SkyChange phase;
    public Vector3 noon;
    public bool dayTime = true;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve LightingIntensityMult;
    public AnimationCurve reflectionIntesityMult;

    void Start ()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    void Update ()
    {
        if(phase.bloodphase) // when normal day
        {
            time += timeRate * Time.deltaTime;

            if(time >= 1.0f)
                time = 0f;

            // rotation
            sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
            moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

            // intensity
            sun.intensity = sunIntensity.Evaluate(time);
            moon.intensity = moonIntensity.Evaluate(time);

            // color change
            sun.color = sunColor.Evaluate(time);
            moon.color = moonColor.Evaluate(time);

            // sunset
            if (time > 0.80f && time < 0.81f){
                moon.gameObject.SetActive(true);
                dayTime = false;
            // sunrise
            } else if (time > 0.30f && time < 0.31f){
                moon.gameObject.SetActive(false);
                dayTime = true;
            }
        }
        else // bloodphase
        {
            moon.gameObject.SetActive(false);
            sun.gameObject.SetActive(false);
        }  
    }
}
