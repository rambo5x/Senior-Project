using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
     public void QuitGame() 
    {
        Application.Quit ();
        Debug.Log("Game is exiting");
    }
}
