using UnityEngine;

public class BossEnemy : EnemyBase
{
    void Start()
    {
        Debug.Log("Hard Enemy Spawned");
        Speed = 0.25f; // Boss enemies are Slow but hard to move
    }
}
