using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    private Rigidbody _enemyRb;
    private GameObject _player;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    private void Awake()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player GameObject not found! Make sure your Player GameObject is tagged 'Player'.");
        }
    }

    public void MoveTowardsPlayer()
    {
        if (_player != null)
        {
            Vector3 lookDirection = (_player.transform.position - transform.position).normalized;
            _enemyRb.AddForce(lookDirection * _speed);
        }
    }
}
