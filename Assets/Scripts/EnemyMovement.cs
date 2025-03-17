using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody _enemyRb;
    private GameObject _player;
    private EnemyBase _enemyBase;

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
            _enemyRb.AddForce(lookDirection * _enemyBase.Speed);
        }
    }
}
