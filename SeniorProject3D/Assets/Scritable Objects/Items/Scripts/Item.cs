using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;

    public virtual void Use()
    {
        Debug.Log(Name + " was used.");
    }
}
