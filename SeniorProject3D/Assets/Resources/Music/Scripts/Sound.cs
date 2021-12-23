using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
   public AudioClip clip;
   [Range(0f,1f)]
   public float volume;
   [Range(.1f,3f)]
   public float pitch;
   public bool loop;

   [HideInInspector]
   public AudioSource source;
   
   // drop this in for desired sound effect wherever it is triggered.
   // FindObjectOfType<AudioManager>().Play("FILENAMEHERE"); 
}