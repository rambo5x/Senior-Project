using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    [SerializeField] public List<AudioClip> audioClips = new List<AudioClip>();
    [System.NonSerialized] public Dictionary<string, int> audioClipMap = new Dictionary<string, int>();
    [System.NonSerialized] private static AudioMaster _instance;
    

    public static AudioMaster Instance { get {return _instance; } }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        for (int i = 0; i < audioClips.Count; i++){
            audioClipMap.Add(audioClips[i].name, i);
        }
    }

    public AudioClip GetAudioClip(string clipName){
        return audioClips[audioClipMap[clipName]];
    }
}
