using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 18f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 2.0f;
    [SerializeField] private bool pierce = false;

    private Rigidbody2D _rb;
    private PoolMember _poolMember;
    private float _timer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _poolMember = GetComponent<PoolMember>();
    }

    private void OnEnable()
    {
        _timer = lifeTime;
        _rb.linearVelocity = Vector2.up * speed;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            _poolMember.ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Asteroid")) return;

        var asteroid = other.GetComponent<Asteroid>();
        if (asteroid != null)
            asteroid.Hit(damage);

        if (!pierce)
            _poolMember.ReturnToPool();
    }
}
