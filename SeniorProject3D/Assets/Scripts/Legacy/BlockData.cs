using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockDictionary {
    Brickwall,
    Bush,
    Dirt,
    Glass,
    Grass,
    Ice,
    Sand,
    Snow,
    Stone,
    Tree,
    Upper_Dirt,
    Watermelon
}

public class BlockData : MonoBehaviour
{
    public BlockDictionary id;
    public Vector3 pos;
}
