using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GunType { PISTOL, RIFLE, SHOTGUN, SNIPER }
public class Gun : Weapon
{
    public UIManager UI;

    [Header("Gun Properties")]
    [SerializeField] public bool isAutomatic = false;
    [SerializeField] public bool canSwapFiringMode = false;
    [SerializeField] public GunType gunType = GunType.PISTOL;
    [SerializeField] public bool hasFlashLight = true;
    [SerializeField] public bool flashLightOn = false;

    [Header("Shoot Properties")]
    [SerializeField] public float damage = 10f;
    [SerializeField] public float range = 100f;
    [SerializeField] public float fireRate = 15f;
    [SerializeField] public float impactForce = 30f;
    [SerializeField] private float nextTimetoFire = 0f;

    [Header("Ammo Properties")]
    public int maxAmmo = 10;
     public int currentAmmo;
    [SerializeField] public float reloadTime = 1f;

    [Header("Gun Attachables")]
    [SerializeField] public Camera fpsCam;
    [SerializeField] public ParticleSystem mf;
    [SerializeField] public GameObject[] impactEffects;
    private enum ImpactEffect{ DEFAULT, BLOOD }
    [SerializeField] public Animator animator;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private bool canZoom = true;
    [System.NonSerialized] private bool isScoped = false;
    [SerializeField] private bool isSniper;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] public GameObject scopedOverlay;
    [SerializeField] public GameObject nightVisionFilter;
    [SerializeField] public GameObject nightVisionLight;
    [SerializeField] public GameObject crossHair;
    [SerializeField] private GameObject weaponCam;
    private AudioSource audio;
    [System.NonSerialized] private bool isReloading = false;
    private AudioClip fireSound, reloadSound, emptySound;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Misc.")]
    [SerializeField] public GameObject weaponsHolder;
    [SerializeField] public GameObject flashLight;


    void Awake()
    {
        defaultFOV = fpsCam.fieldOfView;
    }

    void Start()
    {
        UI = GameObject.FindGameObjectWithTag("UISystem").GetComponent<UIManager>();
        UI.SetAmmo(currentAmmo + "/" + maxAmmo);
        if(currentAmmo <= 0)
        {
            currentAmmo = maxAmmo;
        }
        audio = GetComponent<AudioSource>();

        switch(gunType){
            case GunType.PISTOL:
                fireSound = AudioMaster.Instance.GetAudioClip("pistol");
                break;
            case GunType.RIFLE:
                fireSound = AudioMaster.Instance.GetAudioClip("rifle");
                break;
            case GunType.SHOTGUN:
                fireSound = AudioMaster.Instance.GetAudioClip("shotgun");
                break;
            case GunType.SNIPER:
                fireSound = AudioMaster.Instance.GetAudioClip("cg1");
                break;
        }
        reloadSound = AudioMaster.Instance.GetAudioClip("weapswitch");
        emptySound = AudioMaster.Instance.GetAudioClip("click");
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }
   
    void Update()
    {
        if(isReloading)
            return;

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            UI.SetAmmo(currentAmmo + "/" + maxAmmo);
            return;
        }
        UI.SetAmmo(currentAmmo + "/" + maxAmmo);
        
        if(Input.GetKeyDown(KeyCode.V) && canSwapFiringMode){
            isAutomatic = !isAutomatic;
        }

        if (!isScoped){
            if (Input.GetKeyDown(KeyCode.F) && hasFlashLight){
                flashLightOn = !flashLightOn;
                if (gunType != GunType.SNIPER) flashLight.GetComponent<Light>().enabled = flashLightOn;
            }
        }
        
        if((isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1")) && Time.time >= nextTimetoFire && currentAmmo > 0) //get button down for single shooting and without for automatic
        {
            nextTimetoFire = Time.time + 1f/fireRate; //shoot in intervals of 1.25 seconds
            Shoot();
        }

        if (Input.GetButtonDown("Fire1") && currentAmmo <= 0) audio.PlayOneShot(emptySound);

        if(canZoom)
        {
            HandleZoom();
        }
    }

    private void HandleZoom() //handle starting and ending coroutine for zooming
    {
        if(Input.GetKeyDown(zoomKey))
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }
        if(Input.GetKeyUp(zoomKey))
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = fpsCam.fieldOfView;
        float timeElapsed = 0;

        isScoped = isEnter;

        if(isSniper)
        {
            animator.SetBool("Scoped", isScoped);

            if (!isScoped){
                crossHair.SetActive(!isScoped);
                scopedOverlay.GetComponent<Image>().enabled = isScoped;
                if (hasFlashLight && flashLightOn) {
                    nightVisionFilter.GetComponent<Image>().enabled = isScoped;
                    nightVisionLight.GetComponent<Light>().enabled = isScoped;
                }
                weaponCam.SetActive(!isScoped);
                ToggleSniperMesh(true);
            }
        }
        
        while(timeElapsed < timeToZoom)
        {
            fpsCam.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if(isSniper && isScoped)
        {
            crossHair.SetActive(!isScoped);
            scopedOverlay.GetComponent<Image>().enabled = isScoped;
            if (flashLightOn) {
                nightVisionFilter.GetComponent<Image>().enabled = isScoped;
                nightVisionLight.GetComponent<Light>().enabled = isScoped;
            }
            weaponCam.SetActive(!isScoped);
            ToggleSniperMesh(false);
        }

        fpsCam.fieldOfView = targetFOV;
        zoomRoutine = null; //reset routine
        weaponsHolder.GetComponent<SwitchWeapon>().weaponSwitchingEnabled = !isScoped;

        void ToggleSniperMesh(bool isEnabled){
            gameObject.GetComponent<MeshRenderer>().enabled = isEnabled;
            gameObject.GetComponent<MeshCollider>().enabled = isEnabled;
            for (int i = 0; i < 4; i++) {
                gameObject.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = isEnabled;
                gameObject.transform.GetChild(i).GetComponent<MeshCollider>().enabled = isEnabled;
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        //Debug.Log("Reloading...");

        animator.SetBool("Reloading", true);
        audio.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime - .25f);
        
        animator.SetBool("Reloading", false);

        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        mf.Play();

        currentAmmo--;
        UI.SetAmmo(currentAmmo + "/" + maxAmmo);
        audio.PlayOneShot(fireSound);

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);

            //change was this line 
            Target target = hit.transform.GetComponent<Target>();
            GameObject impactIE;
            if(target != null)
            {
                target.TakeDamage(damage);
                impactIE = Instantiate(impactEffects[(int)ImpactEffect.BLOOD], hit.point, Quaternion.LookRotation(hit.normal));
            } 
            else 
            {
                impactIE = Instantiate(impactEffects[(int)ImpactEffect.DEFAULT], hit.point, Quaternion.LookRotation(hit.normal));
            }

            if(hit.rigidbody != null) 
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
            Destroy(impactIE, 2f);
        }
    }
}
