using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int _waveNumber = 1;
    [SerializeField] private int _maxEnemiesToSpawn = 5;

    [Header("Enemies")]
    [SerializeField] private float _spawnMinRange = 7.0f;
    [SerializeField] private float _spawnMaxRange = 11.0f;
    [SerializeField] private GameObject _easyEnemyPrefab;
    [SerializeField] private GameObject _normalEnemyPrefab;
    [SerializeField] private GameObject _hardEnemyPrefab;
    [SerializeField] private GameObject _bossEnemyPrefab;
    private float _bossSpawnYOffset = 2.1f;
    private int _enemyCount;

    [Header("Power-Ups")]
    [SerializeField] private GameObject _powerUpPrefab;
    [SerializeField] private GameObject _rocketPowerUpPrefab;
    [SerializeField] private GameObject _powerJumpPowerUpPrefab;
    [SerializeField] private GameObject _invisibilityPowerUpPrefab;
    private GameObject[] powerUpPrefabs;
    [SerializeField] private int _maxPowerUpsInScene = 4;

    private int _score = 0;
    private GameHUDController _gameHUDController;
    private Queue<GameObject> _pendingEnemies = new Queue<GameObject>();

    public int WaveNumber
    {
        get { return _waveNumber; }
    }

    public int Score
    {
        get { return _score; }
        protected set { _score = value; }
    }

    public string[] PowerUpPrefabNames
    {
        get
        {
            if (powerUpPrefabs != null)
            {
                string[] names = new string[powerUpPrefabs.Length];
                for (int i = 0; i < powerUpPrefabs.Length; i++)
                {
                    if (powerUpPrefabs[i] != null)
                    {
                        names[i] = powerUpPrefabs[i].name.Replace("PowerUpPrefab", "");
                    }
                }
                return names;
            }
            else
            {
                return new string[0];
            }
        }
    }

    void Start()
    {
        powerUpPrefabs = new GameObject[] { _powerUpPrefab, _rocketPowerUpPrefab, _powerJumpPowerUpPrefab, _invisibilityPowerUpPrefab };
        _score = 0;
        StartNewWave();

        _gameHUDController = FindFirstObjectByType<GameHUDController>();
        if (_gameHUDController == null)
        {
            Debug.LogError("GameHUDController not found in the scene!");
        }
    }

    void Update()
    {
        _enemyCount = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None).Length;
        while (_enemyCount < _maxEnemiesToSpawn && _pendingEnemies.Count > 0)
        {
            SpawnEnemy(_pendingEnemies.Dequeue(), 20f);
            _enemyCount++;
        }
        if (_enemyCount == 0 && _pendingEnemies.Count == 0)
        {
            _waveNumber++;
            StartNewWave();
        }
    }

    void StartNewWave()
    {
        _pendingEnemies.Clear();

        SpawnWave(_waveNumber);
        if (_waveNumber == 1)
        {
            if (_gameHUDController != null)
            {
                _gameHUDController.AnnounceWave("Wave " + _waveNumber);
            }
        }
        else if (_waveNumber % 10 == 0)
        {
            if (_gameHUDController != null)
            {
                _gameHUDController.AnnounceWave("Boss Wave");
            }
        }
        else
        {
            if (_gameHUDController != null)
            {
                _gameHUDController.AnnounceWave("Wave " + _waveNumber);
            }
        }
        if (_waveNumber % 2 == 0)
        {
            SpawnPowerUp();
        }
    }

    void SpawnWave(int waveNumber)
    {
        Debug.Log($">>> Wave {waveNumber}");
        bool spawnBoss = (waveNumber % 10 == 0);

        //calculate enemies
        int easyCount = (waveNumber - 1) % 4 + 1;
        int normalCount = (waveNumber >= 5) ? (waveNumber - 5) / 4 + 1 : 0;
        int hardCount = (waveNumber >= 21) ? ((waveNumber - 21) / 16) + 1 : 0;

        List<GameObject> enemiesToSpawn = new List<GameObject>();
        int initialSpawnCount = _maxEnemiesToSpawn;

        for (int i = 0; i < easyCount; i++) enemiesToSpawn.Add(_easyEnemyPrefab);
        for (int i = 0; i < normalCount; i++) enemiesToSpawn.Add(_normalEnemyPrefab);
        for (int i = 0; i < hardCount; i++) enemiesToSpawn.Add(_hardEnemyPrefab);

        if (spawnBoss)
        {
            Debug.Log("BOSS APPEARS!");
            SpawnEnemy(_bossEnemyPrefab, _bossSpawnYOffset);
            _enemyCount += 3;
            initialSpawnCount = _maxEnemiesToSpawn - 3;
        }

        for (int i = 0; i < Mathf.Min(initialSpawnCount, enemiesToSpawn.Count); i++)
        {
            SpawnEnemy(enemiesToSpawn[i], 20f);
            _enemyCount++;
        }

        for (int i = initialSpawnCount; i < enemiesToSpawn.Count; i++)
        {
            _pendingEnemies.Enqueue(enemiesToSpawn[i]);
        }
    }

    private void SpawnEnemy(GameObject prefabToSpawn, float yOffset)
    {
        Instantiate(prefabToSpawn, GenerateSpawnPosition(yOffset), prefabToSpawn.transform.rotation);
    }

    private void SpawnPowerUp()
    {
        GameObject[] existingPowerUps = GameObject.FindGameObjectsWithTag("PowerBoostPowerUp")
            .Concat(GameObject.FindGameObjectsWithTag("RocketPowerUp"))
            .Concat(GameObject.FindGameObjectsWithTag("PowerJumpPowerUp"))
            .Concat(GameObject.FindGameObjectsWithTag("InvisibilityPowerUp"))
            .ToArray();
        if (existingPowerUps.Length < _maxPowerUpsInScene)
        {
            Vector3 spawnPos = GenerateSpawnPosition(0f);
            int powerUpIndex = Random.Range(0, powerUpPrefabs.Length);
            Instantiate(powerUpPrefabs[powerUpIndex], spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("Maximum number of power-ups reached (" + _maxPowerUpsInScene + "). Not spawning a new one.");
        }
    }

    private Vector3 GenerateSpawnPosition(float yOffset)
    {
        float spawnPosX = Random.Range(-_spawnMaxRange, _spawnMaxRange);
        float spawnPosZ;
        if (Mathf.Abs(spawnPosX) > _spawnMinRange)
        {
            spawnPosZ = Random.Range(-_spawnMinRange, _spawnMinRange);
        }
        else
        {
            spawnPosZ = Random.Range(-_spawnMaxRange, _spawnMaxRange);
        }

        return new Vector3(spawnPosX, yOffset, spawnPosZ);
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;
        if (_gameHUDController != null)
        {
            _gameHUDController.UpdateScore(Score);
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Player fell off.");
        int finalScore = Score;
        GameOverData.FinalScore = finalScore;
        SceneManager.LoadScene("GameOverScene");
    }
}
