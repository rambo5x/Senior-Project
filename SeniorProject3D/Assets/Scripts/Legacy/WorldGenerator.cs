using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton World Generator class
public class WorldGenerator : MonoBehaviour
{
    private static WorldGenerator _instance;
    public static int maxWidth = 32;
    public static int maxHeight = 32;
    public static int maxDepth = 32;
    public GameObject[] blockPrefabs;

    private Vector3 pos;
    private int seed, amplitudeMulti;
    private float frequencyMulti;

    // 3D array of block GameObjects
    public GameObject[, ,] blocks = new GameObject[maxWidth,maxHeight,maxDepth];

    public static WorldGenerator Instance {get{ return _instance; }}

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start()
    {
        pos = new Vector3(0, 0, 0);

        seed = (int)Random.Range(20, 100); //Allows for a range of translation values to make the world unique every time.
        amplitudeMulti = (int)Random.Range(1.1f, maxHeight); //Allows for height range to be dynamically adjusted. Represents the constant scale to multiply with the perlin noise amplitude to generate the height values.
        frequencyMulti = (int)Random.Range(8, 16); //Allows for stretching an object across a plane. A higher widthMutli value means lower frequency and vice versa. This reprents the hills or bumps.

        /*
        Perlin Noise Implementation:

        In order to properly impelement Perlin Noise, the following factors must be considered:

        Amplitude, Frequency, and Translation

        Intro: 
        The current formula would be P(x,y) where P represents the function created for perlin noise with its parameters as x and y. 
        These parameters represent the dynamically changing values of positioning for creating a vector that will be the position to place a block.

        For Amplitude,

        Steps:
        In the given scenario above, the height would be restricted to a certain range. 
        If we want to adjust this height, multiply P(x,y) by a constant H to give the new formula of P'(x,y) = HP(x,y).
        Where P' is the representation Perlin Noise scaled by the value of H.

        For Frequency,

        Steps: 
        The frequency in the perlin noise creates the bumps within the plane. In order to create larger ones, you can stretch the noise with a transformation value.
        In this case, the formula would now be P'(x,y) = P(x/s, y/s).
        Where P' is a Perlin noise function that is stretched s times in two directions within the plane.
        And a higher s value represents a lower frequency while a lower s value represents a higher frequency.

        For Translation,
        
        Steps:
        Since we are able to stretch the perlin noise in both a horizontal and vertical direction, the last step is to add translation.
        In this case, the formula would now be P'(x,y) = P(x - x0, y - y0).
        Where P' is a Perlin noise function that is translated across the current x position minus the initial.
        And if the translation needed to be in a positive direction, the formula would be adjusted as P'(x,y) = P(x + x0, y + y0).

        Conclusion:
        In summary, we are now able to implement perlin noise with the following parameters of amplitude, frequency, and translation.
        The finalized formula with all factors accounted for is:
        P'(x,y) = ⅀n HnPn((x - xn) / sn(y - yn) / sn)
        */

        for (int depth = 0; depth < maxDepth; depth++){
            for (int width = 0; width < maxWidth; width++){
                int columnMaxHeight = (int)(Mathf.PerlinNoise((width + seed) / frequencyMulti, (depth + seed) / frequencyMulti) * amplitudeMulti); //Implementation of Perlin Noise with step explanation above.
                GenerateColumn(width, columnMaxHeight, depth);
                Debug.Log("perlin noise: " + columnMaxHeight + " amplitudeMulti: " + amplitudeMulti + " frequencyMulti: " + frequencyMulti + " seed: " + seed);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.ClearDeveloperConsole();
        }

        if (Input.GetKeyDown(KeyCode.O)){
            DebugTools.DisplayBlockData();
        }
    }

    private void GenerateColumn(int width, int maxHeight, int depth){
        // Generate top grass block
        pos = new Vector3(width, maxHeight, depth);
        blocks[width, maxHeight, depth] = GenerateBlock(BlockDictionary.Grass, pos);

        // Generate dirt
        int numDirt = (int) Random.Range(2,4);
        for (int i = 0; i < numDirt; i++){
            if (maxHeight == 0) return;
            maxHeight--;
            pos = new Vector3(width, maxHeight, depth);
            blocks[width, maxHeight, depth] = GenerateBlock(BlockDictionary.Dirt, pos);
        }

        // Generate stone
        while (maxHeight > 0){
            maxHeight--;
            pos = new Vector3(width, maxHeight, depth);
            blocks[width, maxHeight, depth] = GenerateBlock(BlockDictionary.Stone, pos);
        }
    }

    private GameObject GenerateBlock(BlockDictionary blockId, Vector3 currentPos){
        GameObject block = Instantiate(blockPrefabs[(int) blockId], pos, Quaternion.identity);
        BlockData blockData = block.AddComponent<BlockData>();
        blockData.id = blockId;
        blockData.pos = currentPos;
        return block;
    }
}