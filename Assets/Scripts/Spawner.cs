using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWavenumber;
    int enemiesReaminingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }
    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance); //este considerat camper daca nu s-a miscat mai mult decta distanta minima data de threshold
                campPositionOld = playerT.position;
            }

            if (enemiesReaminingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesReaminingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy() //pt a spawna inamicul si a seta culoarea in care va palpai placa unde va fi spawnat inamicul
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        Transform spawnTile = map.GetRandomOpenTile();
        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColour = tileMat.color;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        if (isCamping)
        {
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1)); //incerc sa repar sa palpaie placa cand se spawneaza inamicul fix langa player!! daca nu merge poate fi sters
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawneedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawneedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void OnEnemyDeath ()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive ==0)
        {
            NextWave();
        }
    }


    void NextWave ()
    {
        currentWavenumber ++;
        
        if (currentWavenumber - 1 < waves.Length)
        {
            currentWave = waves[currentWavenumber - 1];

            enemiesReaminingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesReaminingToSpawn;
        }

        if (OnNewWave != null)
        {
            OnNewWave(currentWavenumber);
        }
        ResetPlayerPosition();
    }

    [System.Serializable]

    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
