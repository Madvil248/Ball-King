using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private float _spawnRange = 9.0f;
    [SerializeField] private GameObject _easyEnemyPrefab;
    [SerializeField] private GameObject _normalEnemyPrefab;
    [SerializeField] private GameObject _hardEnemyPrefab;
    [SerializeField] private GameObject _bossEnemyPrefab;

    [Header("Power-Ups")]
    [SerializeField] private GameObject _powerUpPrefab;
    [SerializeField] private GameObject _rocketPowerUpPrefab;
    [SerializeField] private GameObject _powerJumpPowerUpPrefab;
    //[SerializeField] private GameObject _invisibilityPowerUpPrefab;

    private int _enemyCount;
    [SerializeField] private int _waveNumber = 1;

    // Wave Definitions - Hardcoded wave compositions
    [System.Serializable]
    public class WaveConfig
    {
        public int waveNumber;
        public int easyEnemies;
        public int normalEnemies;
        public int hardEnemies;
        public bool spawnBoss;
    }

    public List<WaveConfig> waves = new List<WaveConfig>()
    {
        new WaveConfig { waveNumber = 1, easyEnemies = 1, normalEnemies = 0, hardEnemies = 0, spawnBoss = false },
        new WaveConfig { waveNumber = 2, easyEnemies = 2, normalEnemies = 0, hardEnemies = 0, spawnBoss = false },
        new WaveConfig { waveNumber = 3, easyEnemies = 1, normalEnemies = 1, hardEnemies = 0, spawnBoss = false },
        new WaveConfig { waveNumber = 4, easyEnemies = 2, normalEnemies = 2, hardEnemies = 0, spawnBoss = false },
        new WaveConfig { waveNumber = 5, easyEnemies = 3, normalEnemies = 1, hardEnemies = 1, spawnBoss = false },
        new WaveConfig { waveNumber = 6, easyEnemies = 3, normalEnemies = 2, hardEnemies = 2, spawnBoss = false },
        new WaveConfig { waveNumber = 7, easyEnemies = 3, normalEnemies = 0, hardEnemies = 0, spawnBoss = true },
        new WaveConfig { waveNumber = 8, easyEnemies = 0, normalEnemies = 3, hardEnemies = 2, spawnBoss = false },
        new WaveConfig { waveNumber = 9, easyEnemies = 0, normalEnemies = 3, hardEnemies = 3, spawnBoss = false },
        new WaveConfig { waveNumber = 10, easyEnemies = 4, normalEnemies = 3, hardEnemies = 2, spawnBoss = false },
        new WaveConfig { waveNumber = 11, easyEnemies = 4, normalEnemies = 3, hardEnemies = 3, spawnBoss = false },
        new WaveConfig { waveNumber = 12, easyEnemies = 2, normalEnemies = 1, hardEnemies = 1, spawnBoss = true }
    };

    public int WaveNumber
    {
        get { return _waveNumber; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnEnemyWave();
        SpawnPowerUp();
    }

    // Update is called once per frame
    void Update()
    {
        _enemyCount = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None).Length;
        if (_enemyCount == 0)
        {
            _waveNumber++;
            if (_waveNumber <= waves.Count)
            {
                SpawnEnemyWave();
                if (_waveNumber % 2 == 0)
                {
                    SpawnPowerUp();
                }
            }
            else
            {
                Debug.Log("--- ALL WAVES COMPLETED! --- Game Win Condition Here!");
                // Implement Game Win condition here (e.g., show win screen, stop spawning, etc.)
                enabled = false; // Disable SpawnManager to stop further updates
            }

        }
    }

    void SpawnEnemyWave()
    {
        if (_waveNumber > waves.Count)
        {
            return;
        }

        WaveConfig currentWave = waves[_waveNumber - 1];
        Debug.Log($"--- Starting Wave {currentWave.waveNumber} ---");
        SpawnEnemies(currentWave.easyEnemies, _easyEnemyPrefab, "Easy"); // Use new generic SpawnEnemies
        SpawnEnemies(currentWave.normalEnemies, _normalEnemyPrefab, "Normal"); // Use new generic SpawnEnemies
        SpawnEnemies(currentWave.hardEnemies, _hardEnemyPrefab, "Hard"); // Use new generic SpawnEnemies

        if (currentWave.spawnBoss)
        {
            SpawnBossEnemy();
            Debug.LogWarning($"*** BOSS WAVE {currentWave.waveNumber} SPAWNED! ***");
        }
    }

    private void SpawnEnemies(int count, GameObject prefabToSpawn, string enemyTypeName)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Spawning {enemyTypeName} Enemy");
            Instantiate(prefabToSpawn, GenerateSpawnPosition(), prefabToSpawn.transform.rotation);
        }
    }

    private void SpawnBossEnemy()
    {
        Debug.LogWarning("--- Spawning BOSS Enemy ---");
        Instantiate(_bossEnemyPrefab, GenerateSpawnPosition(), _bossEnemyPrefab.transform.rotation);
    }

    private void SpawnPowerUp()
    {
        float randomPowerUp = Random.value;
        if (randomPowerUp < 0.33f)
        {
            Debug.Log("Spawning Speed Boost PowerUp");
            Instantiate(_powerUpPrefab, GenerateSpawnPosition(), _powerUpPrefab.transform.rotation);
        }
        else if(randomPowerUp < 0.66f)
        {
            Debug.Log("Spawning Rocket PowerUp");
            Instantiate(_rocketPowerUpPrefab, GenerateSpawnPosition(), _rocketPowerUpPrefab.transform.rotation);
        }
        else
        {
            Debug.Log("Spawning Power Jump PowerUp");
            Instantiate(_powerJumpPowerUpPrefab, GenerateSpawnPosition(), _powerJumpPowerUpPrefab.transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-_spawnRange, _spawnRange);
        float spawnPosZ = Random.Range(-_spawnRange, _spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}
