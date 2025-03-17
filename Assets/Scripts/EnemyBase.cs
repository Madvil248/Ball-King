using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] protected float _pushResistance = 1.0f;
    private Rigidbody _enemyRb;
    private GameObject _player;

    [Header("Score")]
    [SerializeField] private int _scoreValue = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip _collisionSound;
    private AudioSource _audioSource;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public float PushResistance
    {
        get { return _pushResistance; }
        set { _pushResistance = value; }
    }

    protected virtual void Awake()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player GameObject not found! Make sure your Player GameObject is tagged 'Player'.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        EnemyUpdate();
    }

    public virtual void MoveTowardsPlayer()
    {
        if (_player != null)
        {
            Vector3 lookDirection = (_player.transform.position - transform.position).normalized;
            _enemyRb.AddForce(lookDirection * _speed);
        }
    }

    public virtual void EnemyUpdate()
    {
        MoveTowardsPlayer();

        if (transform.position.y < -10)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log("EnemyBase Died!");
        SpawnManager spawnManager = FindFirstObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.IncreaseScore(_scoreValue);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")) && _collisionSound != null)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.PlayOneShot(_collisionSound, 0.5f);
        }
    }
}
