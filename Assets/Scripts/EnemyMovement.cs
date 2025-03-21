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

        _enemyBase = GetComponent<EnemyBase>();
        if (_enemyBase == null)
        {
            Debug.LogError("EnemyBase component not found on this enemy!");
        }
    }

    public void MoveTowardsPlayer()
    {
        if (_player != null && _enemyBase != null)
        {
            Vector3 lookDirection = (_player.transform.position - transform.position).normalized;
            float currentSpeed = _enemyBase.Speed;

            if (GameSettings.Instance != null)
            {
                switch (GameSettings.Instance.CurrentDifficulty)
                {
                    case GameSettings.Difficulty.Easy:
                        currentSpeed += 1f;
                        break;
                    case GameSettings.Difficulty.Normal:
                        currentSpeed += 5f;
                        break;
                    case GameSettings.Difficulty.Hard:
                        currentSpeed += 10f;
                        break;
                }
            }

            _enemyRb.AddForce(lookDirection * currentSpeed);
        }
    }
}
