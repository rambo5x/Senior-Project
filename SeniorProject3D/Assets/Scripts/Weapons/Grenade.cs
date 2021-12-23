using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    [Header("Attributes")]
    [SerializeField] public float delay = 3f;
    [SerializeField] public float force = 700f;
    [SerializeField] public float throwForce = 40f;
    [SerializeField] public float radius = 5f;
    [SerializeField] public float dmg = 50f;
    [SerializeField] public bool isExplodable = true;
    [SerializeField] public bool lightOnThrow = false;
    private Rigidbody theRB;
    public GameObject grenade;
    public GameObject boomEffect;
    private float countDown;
    bool hasExploded = false, hasThrown = false;

    void Awake()
    {
        theRB = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        countDown = delay;
        theRB.constraints = RigidbodyConstraints.FreezeAll;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !hasThrown)
        {
            theRB.constraints = RigidbodyConstraints.None;
            ThrowGrenade();
            hasThrown = true;
        }

        if(hasThrown)
        {
            if (isExplodable){
                countDown -= Time.deltaTime;
                if(countDown <= 0f && !hasExploded)
                {
                    Boom();
                    hasExploded = true;
                }
            }
            if (lightOnThrow && !hasExploded){
                gameObject.transform.GetChild(0).GetComponent<Light>().enabled = true;
                UIManager.Instance.RemoveWeapon(gameObject);
                hasExploded = true;
            }
        }
    }
    void Boom()
    {
        Instantiate(boomEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearObject in colliders)
        {
            Rigidbody rb = nearObject.GetComponent<Rigidbody>();
            Target target = nearObject.transform.GetComponent<Target>();
            if(rb != null && target != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
                target.TakeDamage(dmg);
            }
        }
        UIManager.Instance.RemoveWeapon(gameObject);
        Destroy(gameObject);
    }

    void ThrowGrenade()
    {
        grenade.transform.parent = null;
        theRB.AddForce(-transform.right * throwForce, ForceMode.VelocityChange);
        theRB.useGravity = true;
    }
}
