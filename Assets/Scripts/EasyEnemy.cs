using UnityEngine;

public class EasyEnemy : EnemyBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Easy Enemy Spawned");
        Speed = 2.0f; // Example: Easy enemies are a bit slower
    }
}
