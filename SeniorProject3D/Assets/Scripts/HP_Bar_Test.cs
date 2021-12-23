using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HP_Bar_Test : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public Health_Bar health;
    public bool takedmg;
    float time;
    public float iframe = 2f;
    private AudioClip deathsound;
    private AudioClip damagesound;
    private AudioSource audioSource;
    private bool initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        takedmg = true;
        time  = iframe;
        deathsound = (AudioClip)Resources.Load("Death");
        damagesound = (AudioClip)Resources.Load("Damage");
    }

    void OnTriggerEnter (Collider touch)
    {
        if(touch.gameObject.tag == "Enemy" || touch.gameObject.tag == "Shadow")
        {
            Damage(10);
            audioSource.clip = damagesound;
            audioSource.Play();
        }
        if(touch.gameObject.tag == "Final Boss" || touch.gameObject.tag == "Final Boss Dmg")
        {
            Damage(50);
        }

        if (touch.gameObject.tag == "Explosion" || touch.gameObject.tag == "Buffed Enemy")
        {
            Damage(30);
        }

        if(touch.gameObject.tag == "HealthPack")
        {
            Heal(30);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if (initialized) {
            if(currentHealth <= 0)
            {
                // game over scene here
                Debug.Log("You are dead");
                if (audioSource != null){
                    audioSource.clip = deathsound;
                    audioSource.Play();
                }
                SceneManager.LoadScene(3);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if(!takedmg)
            {
                if (time > 0)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    takedmg = true;
                    Debug.Log("Immunity Off");
                    time = iframe;
                }
            }
        }
    }
    
    void Damage(int damage)
    {
        if(takedmg)
        {
            currentHealth -= damage;
            health.SetHealth(currentHealth);
            takedmg = false;
            Debug.Log("Immunity Granted");
        }
    }

    void Heal(int heal)
    {
        currentHealth += heal;
        if(currentHealth > 100)
        {
            currentHealth = 100;
        }
        health.SetHealth(currentHealth);
    }

    public void InitializeHealth(){
        currentHealth = maxHealth;
        health.SetMaxHealth(maxHealth);
        initialized = true;
    }
}
