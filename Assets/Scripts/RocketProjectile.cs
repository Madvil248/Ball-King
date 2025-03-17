using Unity.VisualScripting;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionForce = 700f;
    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private AudioClip _rocketFireSoundEffect;
    [SerializeField] private AudioClip _explosionSoundEffect;
    [SerializeField] private Vector3 _cameraPosition;

    private Transform _targetEnemy;
    private bool _hasTarget = false;
    private bool _isExploded = false;
    private AudioSource _audioSource;
    private Vector3 _middlePoint = new Vector3(0, 0, 0);

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
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.PlayOneShot(_rocketFireSoundEffect, 0.75f);
    }

    void FixedUpdate()
    {
        if (!_isExploded && _hasTarget && _targetEnemy != null)
        {
            Vector3 directionToTarget = (_targetEnemy.position - transform.position).normalized;

            GetComponent<Rigidbody>().linearVelocity = directionToTarget * _speed;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            GetComponent<Rigidbody>().rotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, Time.deltaTime * 10f);
        }
        else if (!_isExploded && _targetEnemy == null)
        {
            Vector3 directionToTarget = (_middlePoint - transform.position).normalized;

            GetComponent<Rigidbody>().linearVelocity = directionToTarget * _speed;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            GetComponent<Rigidbody>().rotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isExploded) return;

        _isExploded = true;

        if (_explosionEffectPrefab != null)
        {
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        PlayExplosionSound(collision.transform.position);
        Explode();
        Destroy(gameObject);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }
    }

    void PlayExplosionSound(Vector3 vectorPos)
    {
        Debug.Log("PlayExplosionSound()");
        if (_explosionSoundEffect != null && vectorPos != null && _cameraPosition != null)
        {
            Debug.Log("Now should be sound!");
            AudioSource.PlayClipAtPoint(_explosionSoundEffect, _cameraPosition - vectorPos);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
