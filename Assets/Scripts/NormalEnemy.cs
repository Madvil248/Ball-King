using UnityEngine;

public class NormalEnemy : EnemyBase
{
    void Start()
    {
        Debug.Log("Normal Enemy Spawned");
        Speed = 3.0f; // Normal enemies are faster
    }

    public override void TakeDamage(int damageAmount)
    {
        Debug.Log("Normal Enemy took damage! Damage - " + damageAmount);
        base.TakeDamage(damageAmount);
    }
}
