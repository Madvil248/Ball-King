using UnityEngine;

public class BossEnemy : EnemyBase
{
    void Start()
    {
        Debug.Log("Hard Enemy Spawned");
        Speed = 0.25f; // Boss enemies are Slow but hard to move
    }

    public override void TakeDamage(int damageAmount)
    {
        Debug.Log("Boss Enemy took damage!");
        base.TakeDamage(damageAmount);
    }
}
