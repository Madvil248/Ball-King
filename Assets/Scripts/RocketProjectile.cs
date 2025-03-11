using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionForce = 700f;
    [SerializeField] private GameObject _explosionEffectPrefab;

    private Transform _targetEnemy;
    private bool _hasTarget = false;
    private bool _isExploded = false;
    public void SetTarget(Transform target)
    {
        _targetEnemy = target;
        _hasTarget = true;
    }

    void Start()
    {
        if (!_hasTarget)
        {
            Debug.LogWarning("Rocket launched without a target! Destroying.");
            Destroy(gameObject); // Destroy if launched without a target
            return;
        }
    }

    void FixedUpdate()
    {
        if (!_isExploded && _hasTarget && _targetEnemy != null)
        {
            //direction to target
            Vector3 directionToTarget = (_targetEnemy.position - transform.position).normalized;

            //Move towards the target using Rigidbody.velocity for physics-based movement
            GetComponent<Rigidbody>().linearVelocity = directionToTarget * _speed;

            //Make the rocket orient itself to face the direction of travel
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            GetComponent<Rigidbody>().rotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
        else
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Stop moving if no target or exploded
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isExploded) return;

        _isExploded = true;

        //Explosion Effect
        if (_explosionEffectPrefab != null)
        {
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        //Explosion Force to nearby enemies
        Explode();
        //Destroy the rocket projectile
        Destroy(gameObject);
    }

    void Explode()
    {
        // Find all colliders in explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply explosion force
                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
