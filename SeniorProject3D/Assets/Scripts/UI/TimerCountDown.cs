using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCountDown : MonoBehaviour
{
  public GameObject textDisplay;
  public bool takingAway = false;
  public SkyChange skyChange;

  void Start()
  {
      textDisplay.GetComponent<Text>().text = "Time Until Boss Night: " + Mathf.RoundToInt(skyChange.timeRemaining);
  }

  void Update()
  {
      if (!skyChange.bloodphase){
          textDisplay.GetComponent<Text>().text = "Time Until Boss Night: NOW";
      }
      else if(takingAway == false && skyChange.timeRemaining > 0)
      {
          StartCoroutine(TimerTake());
      }
  }
  IEnumerator TimerTake()
  {
      takingAway = true;
      yield return new WaitForSeconds(1);
      textDisplay.GetComponent<Text>().text = "Time Until Boss Night: " + Mathf.RoundToInt(skyChange.timeRemaining);
      takingAway = false;
  }
}
