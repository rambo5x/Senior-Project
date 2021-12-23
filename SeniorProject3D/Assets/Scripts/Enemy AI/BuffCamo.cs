using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCamo : MonoBehaviour
{
    float alpha;
    Material[] myMaterials;
    int range;
    bool invis;

    // Start is called before the first frame update
    void Start()
    {
        myMaterials = gameObject.GetComponent<Renderer>().materials;
        range = myMaterials.Length;
        invis = true;
        alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // turn invis (Buffed is always invis)
        if(invis)
        {   
            for (int m = 0; m < range; m++)
            {
                ChangeAlpha(myMaterials[m], alpha);
            }
        }
    }

    // change to invisible and back: 
    // alpha 0 being invisible & 1 being visible
    void ChangeAlpha(Material mat, float alpha)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        mat.SetColor("_Color", newColor);
    }
}
