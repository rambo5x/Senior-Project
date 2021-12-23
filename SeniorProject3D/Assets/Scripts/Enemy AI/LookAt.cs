using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LookAt : MonoBehaviour
{
    public GameObject Player;
    public float Distance;
    public NavMeshAgent _agent;

    // Teleport Location Trackers
    float enemyX;
    float enemyZ;
    float newX;
    float newZ;
 
    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        this.transform.LookAt(Player.transform);
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);

        if(Distance <= 6f)
        {
            Teleport();
        }
    }

    public void Teleport()
    {
        if(Player.transform.position.z >= this.transform.position.z)
        {
            //positive Z
            newZ = (Player.transform.position.z - this.transform.position.z) * 5;
            newZ += this.transform.position.z;
            newX = (Player.transform.position.x - this.transform.position.x) * 5;
            newX += this.transform.position.x;

            if(newX > 250)
                newX = 250;
            if(newZ > 250)
                newZ = 250;
        }
        if(Player.transform.position.z < this.transform.position.z)
        {
            //negative Z
            newZ = (Player.transform.position.z - this.transform.position.z) * 5;
            newZ += this.transform.position.z;
            newX = (Player.transform.position.x - this.transform.position.x) * 5;
            newX += this.transform.position.x;

            if(newX < 0)
                newX = 0;
            if(newZ < 0)
                newZ = 0;
        }
         // (X,Y,Z)
        this.transform.position = new Vector3(newX, 0.5f, newZ);
    }
}
