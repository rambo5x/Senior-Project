using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour{
    public static void DisplayBlockData(){
        for (int i = 0; i < WorldGenerator.maxWidth; i++){
            for (int j = 0; j < WorldGenerator.maxHeight; j++){
                for (int k = 0; k < WorldGenerator.maxDepth; k++){
                    GameObject block = WorldGenerator.Instance.blocks[i, j, k];
                    if (block != null){
                        BlockData blockData = block.GetComponent<BlockData>();
                        Debug.Log("id: " + blockData.id + ", position: " + blockData.pos);
                    }
                }
            }
        }
    }
}
