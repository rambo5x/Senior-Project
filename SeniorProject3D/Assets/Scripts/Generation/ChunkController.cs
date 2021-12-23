using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChunkController : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] public GameObject terrainObject;
    [SerializeField] public GameObject itemsObject;
    [SerializeField] public GameObject enemiesObject;
    [Header("Object Prefabs")]
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject bossPrefab;
    [SerializeField] public GameObject[] enemyPrefabs;
    [SerializeField] public GameObject[] eliteEnemyPrefabs;
    [SerializeField] public GameObject[] itemPrefabs;
    [SerializeField] public Material textureAtlas;
    [Header("Spawning")]
    [SerializeField] public int enemyCount = 0;
    [SerializeField] public int itemCount = 0;
    [SerializeField] public float spawnTimer = 5.0f;
    [Header("Generation Adjustments")]
    [SerializeField] public float amplitude = 1.25f;
    [SerializeField] public float frequency = 16.0f;
    [SerializeField] public float persistence = 0.5f;
    [SerializeField] public float lacunarity = 1.25f;
    [SerializeField] public float octaves = 4;
    [SerializeField] public float heightScale = 10.0f;
    [System.NonSerialized] public Vector3 playerStartPosition;
    [System.NonSerialized] public GameObject player;
    [System.NonSerialized] public GameObject boss;
    [System.NonSerialized] public const int maxWidth = 256, maxHeight = 64, maxDepth = 256; // maximums of width, height, depth
    [System.NonSerialized] private int currentEnemyIndex = 0, currentItemIndex = 0, currentEliteEnemyIndex = 0;
    [System.NonSerialized] private float resetSpawnTimer;
    [System.NonSerialized] public CreateChunk worldChunk;
    [System.NonSerialized] public bool isBloodMoon = false;
    [System.NonSerialized] private static ChunkController _instance;
    
    public static ChunkController Instance { get {return _instance; } }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        worldChunk = new CreateChunk(terrainObject, textureAtlas, controller: this);
    }

    // Start is called before the first frame update
    void Start()
    {
        resetSpawnTimer = spawnTimer;
        player = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);

        HP_Bar_Test hpBarTest = player.GetComponent<HP_Bar_Test>();
        hpBarTest.health = GameObject.Find("UI/HP_bar").GetComponent<Health_Bar>();
        hpBarTest.InitializeHealth();
        GameObject.Find("UI/HP_bar/Fill").GetComponent<IFrame>().HealthTrack = hpBarTest;

        Gun sniperRifle = player.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Gun>();
        sniperRifle.scopedOverlay = GameObject.Find("UI/NightVisionFilter/scopeOverlay");
        sniperRifle.crossHair = GameObject.Find("UI/Crosshair");
        sniperRifle.nightVisionFilter = GameObject.Find("UI/NightVisionFilter");

        NavMeshSurface surface = terrainObject.GetComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();

        for (int i = 0; i < itemCount; i++){
            int spawnLocationIndex = (int) UnityEngine.Random.Range(0,worldChunk.topBlockPositions.Count - 1);
            Instantiate(itemPrefabs[currentItemIndex], worldChunk.topBlockPositions[spawnLocationIndex], Quaternion.identity).transform.SetParent(itemsObject.transform);
            if (currentItemIndex == itemPrefabs.Length - 1){
                currentItemIndex = 0;
            } else {
                currentItemIndex++;
            }
        }   
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0){
            spawnTimer = resetSpawnTimer;
            int spawnLocationIndex = (int) UnityEngine.Random.Range(0,worldChunk.topBlockPositions.Count - 1);
            // spawn enemy
            GameObject enemy = isBloodMoon ? 
                Instantiate(eliteEnemyPrefabs[currentEliteEnemyIndex], worldChunk.topBlockPositions[spawnLocationIndex], Quaternion.identity) : 
                Instantiate(enemyPrefabs[currentEnemyIndex], worldChunk.topBlockPositions[spawnLocationIndex], Quaternion.identity);
            enemy.transform.SetParent(enemiesObject.transform);

            if (!isBloodMoon){
                switch(currentEnemyIndex){
                    case 0: {
                        enemy.GetComponent<Enemy_AI>().Player = player;
                        break;
                    }
                    case 1: {
                        enemy.GetComponent<Enemy_AI>().Player = player;
                        break;
                    }
                    case 2: {
                        enemy.GetComponent<Explosion>().Player = player;
                        break;
                    }
                    case 3: {
                        enemy.GetComponent<Tele_Enemy>().Player = player;
                        break;
                    }
                    case 4: {
                        enemy.GetComponent<Camo_Enemy>().Player = player;
                        enemy.transform.GetChild(1).GetComponent<Camo>().Player = player;
                        enemy.transform.GetChild(2).GetComponent<Camo>().Player = player;
                        enemy.transform.GetChild(3).GetComponent<Camo>().Player = player;
                        enemy.transform.GetChild(4).GetComponent<Camo>().Player = player;
                        break;
                    }
                }

                if (currentEnemyIndex == enemyPrefabs.Length - 1){
                    currentEnemyIndex = 0;
                } else {
                    currentEnemyIndex++;
                }
            } else {
                switch(currentEliteEnemyIndex){
                    case 0: {
                        enemy.GetComponent<Explosion>().Player = player;
                        break;
                    }
                    case 1: {
                        enemy.GetComponent<Tele_Enemy>().Player = player;
                        break;
                    }
                    case 2: {
                        enemy.GetComponent<Camo_Enemy>().Player = player;
                        break;
                    }
                    case 3: {
                        enemy.GetComponent<Enemy_AI>().Player = player;
                        break;
                    }
                }

                if (currentEliteEnemyIndex == eliteEnemyPrefabs.Length - 1){
                    currentEliteEnemyIndex = 0;
                } else {
                    currentEliteEnemyIndex++;
                }
            }
        }
    }

    public void SpawnBoss(){
        boss = Instantiate(bossPrefab, worldChunk.topBlockPositions[worldChunk.topBlockPositions.Count / 3], Quaternion.identity);
        boss.GetComponent<BossAtks>().Player = player;
        boss.GetComponent<LookAt>().Player = player;
        boss.transform.GetChild(0).GetComponent<BossWeakness>().WeaponHolder = player.transform.GetChild(0).GetChild(0).gameObject;
    }
}
