using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camo : MonoBehaviour
{
    public GameObject Player;
    public float Distance;
    public float alpha = 0.0f;

    Material[] myMaterials;
    int range;
    bool invis;

    // Start is called before the first frame update
    void Start()
    {
        myMaterials = gameObject.GetComponent<Renderer>().materials;
        range = myMaterials.Length;
        invis = true;
    }

    // Update is called once per frame
    void Update()
    {
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);

        // transparent only
        if(Distance >= 7.0f)
        {
            invis = true;
        }
        else 
        {
            invis = false;
        }

        // turn invis
        if(invis)
        {   
            alpha = 0.0f;
            for (int m = 0; m < range; m++)
            {
                ChangeAlpha(myMaterials[m], alpha);
            }
        }

        // Revealed
        if(!invis)
        {   
            alpha = 1.0f;
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
