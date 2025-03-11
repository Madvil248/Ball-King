using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] protected float _pushResistance = 1.0f;
    private Rigidbody _enemyRb;
    private GameObject _player;

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
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();
    }

    protected virtual void Die()
    {
        Debug.Log("EnemyBase Died!");
        Destroy(gameObject);
    }

    public virtual void TakeDamage(int damageAmount)
    {
        // Basic take damage logic - subclasses can extend or override
        // For now, basic enemy doesn't really 'take damage' in a complex way
        Die(); // For now, any damage kills the base enemy immediately
    }
}
