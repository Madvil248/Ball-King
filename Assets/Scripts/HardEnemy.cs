using UnityEngine;

public class HardEnemy : EnemyBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hard Enemy Spawned");
        Speed = 4.0f; // Hard enemies are faster
    }

    public override void TakeDamage(int damageAmount)
    {
        Debug.Log("Hard Enemy took damage!");
        base.TakeDamage(damageAmount);
    }
}
