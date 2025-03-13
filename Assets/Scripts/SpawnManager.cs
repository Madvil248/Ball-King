using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int _waveNumber = 1;

    [Header("Enemies")]
    [SerializeField] private float _spawnRange = 9.0f;
    [SerializeField] private GameObject _easyEnemyPrefab;
    [SerializeField] private GameObject _normalEnemyPrefab;
    [SerializeField] private GameObject _hardEnemyPrefab;
    [SerializeField] private GameObject _bossEnemyPrefab;
    private int _enemyCount;

    [Header("Power-Ups")]
    [SerializeField] private GameObject _powerUpPrefab;
    [SerializeField] private GameObject _rocketPowerUpPrefab;
    [SerializeField] private GameObject _powerJumpPowerUpPrefab;
    [SerializeField] private GameObject _invisibilityPowerUpPrefab;
    private GameObject[] powerUpPrefabs;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private float _waveTextDefaultSize = 60f;
    [SerializeField] private float _waveTextBigSize = 120f;
    [SerializeField] private float _waveTextAnimationDuration = 0.8f;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    private int _score = 0;

    public int WaveNumber
    {
        get { return _waveNumber; }
    }

    public int Score
    {
        get { return _score; }
        protected set { _score = value; }
    }

    void Start()
    {
        powerUpPrefabs = new GameObject[] { _powerUpPrefab, _rocketPowerUpPrefab, _powerJumpPowerUpPrefab, _invisibilityPowerUpPrefab };
        _score = 0;
        UpdateScoreUI();
        StartNewWave();
    }

    void Update()
    {
        _enemyCount = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None).Length;
        if (_enemyCount == 0)
        {
            _waveNumber++;
            StartNewWave();
        }
    }

    void StartNewWave()
    {
        SpawnWave(_waveNumber);
        if (_waveNumber == 1)
        {
            _waveText.text = "Wave " + _waveNumber;
            _waveText.fontSize = _waveTextDefaultSize;
            _waveText.gameObject.SetActive(true);
        }
        else if (_waveNumber % 10 == 0)
        {
            StartCoroutine(AnimateWaveText("Boss Wave"));
        }
        else
        {
            StartCoroutine(AnimateWaveText("Wave: " + _waveNumber));
        }
        // Every 2nd wave, spawn a power-up
        if (_waveNumber % 2 == 0)
        {
            SpawnPowerUp();
        }
    }

    /// Dynamically determines how many enemies to spawn based on wave number.
    void SpawnWave(int waveNumber)
    {
        Debug.Log($">>> Wave {waveNumber}");
        bool spawnBoss = (waveNumber % 10 == 0);

        //Easy enemies
        int easyCount = (waveNumber - 1) % 4 + 1;

        //Normal enemies
        int normalCount = (waveNumber >= 5) ? (waveNumber - 5) / 4 + 1 : 0;

        //Hard enemies
        int hardCount = (waveNumber >= 21) ? ((waveNumber - 21) / 16) + 1 : 0;

        if (spawnBoss)
        {
            Debug.Log("BOSS APPEARS!");
            SpawnEnemy(_bossEnemyPrefab);
        }

        //Spawn calculated number of each enemy type
        SpawnEnemies(easyCount, _easyEnemyPrefab, "Easy");
        SpawnEnemies(normalCount, _normalEnemyPrefab, "Normal");
        SpawnEnemies(hardCount, _hardEnemyPrefab, "Hard");
    }

    /// Spawns a specified number of enemies of a given type.
    private void SpawnEnemies(int count, GameObject prefabToSpawn, string enemyTypeName)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(prefabToSpawn);
        }
    }


    private void SpawnEnemy(GameObject prefabToSpawn)
    {
        Instantiate(prefabToSpawn, GenerateSpawnPosition(), prefabToSpawn.transform.rotation);
    }

    private void SpawnPowerUp()
    {
        Vector3 spawnPos = GenerateSpawnPosition();
        int powerUpIndex = Random.Range(0, powerUpPrefabs.Length);
        Instantiate(powerUpPrefabs[powerUpIndex], spawnPos, Quaternion.identity);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-_spawnRange, _spawnRange);
        float spawnPosZ = Random.Range(-_spawnRange, _spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }

    IEnumerator AnimateWaveText(string waveText)
    {
        _waveText.text = waveText;

        float timeElapsed = 0f;
        float firstDuration = _waveTextAnimationDuration / 4f;
        float secondDuration = _waveTextAnimationDuration - firstDuration;

        _waveText.fontSize = _waveTextBigSize;

        while (timeElapsed < firstDuration)
        {
            float t = timeElapsed / firstDuration;
            float currentSize = Mathf.Lerp(_waveTextBigSize, 125f, t);
            _waveText.fontSize = currentSize;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        while (timeElapsed < secondDuration)
        {
            float t = timeElapsed / secondDuration;
            float currentSize = Mathf.Lerp(125f, _waveTextDefaultSize, t);
            _waveText.fontSize = currentSize;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateScoreUI()
    {
        _scoreText.text = "Score " + _score.ToString();
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;
        UpdateScoreUI();
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Player fell off.");
        int finalScore = Score;
        GameOverData.FinalScore = finalScore;
        SceneManager.LoadScene("GameOverScene");
    }
}
