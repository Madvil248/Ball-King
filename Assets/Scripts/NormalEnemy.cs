using UnityEngine;

public class NormalEnemy : EnemyBase
{
    void Start()
    {
        Debug.Log("Normal Enemy Spawned");
        Speed = 3.0f; // Normal enemies are faster
    }
}
