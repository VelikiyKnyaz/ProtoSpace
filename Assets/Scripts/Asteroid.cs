using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    public static int ActiveCount { get; private set; }

    [Header("HP by size")]
    [SerializeField] private int hpSmall = 1;
    [SerializeField] private int hpMedium = 2;
    [SerializeField] private int hpLarge = 3;

    [Header("Score by size")]
    [SerializeField] private int scoreSmall = 8;
    [SerializeField] private int scoreMedium = 18;
    [SerializeField] private int scoreLarge = 35;

    [Header("Rotation")]
    [SerializeField] private float rotateDegPerSecMin = 30f;
    [SerializeField] private float rotateDegPerSecMax = 120f;

    private Rigidbody2D _rb;
    private PoolMember _poolMember;
    private Camera _cam;

    private AsteroidSpawner _spawner;
    private GameObjectPool _pickupPool;
    private GameObjectPool _explosionPool;

    private float _size;
    private int _hp;
    private float _pickupDropChance;
    private float _rotSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _poolMember = GetComponent<PoolMember>();
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        ActiveCount++;
    }

    private void OnDisable()
    {
        ActiveCount = Mathf.Max(0, ActiveCount - 1);
    }

    public static void ResetCount() => ActiveCount = 0;

    // ✅ Init ahora recibe pickupPool y explosionPool desde escena
    public void Init(AsteroidSpawner spawner, GameObjectPool pickupPool, GameObjectPool explosionPool,
                     float size, Vector2 velocity, float pickupDropChance)
    {
        _spawner = spawner;
        _pickupPool = pickupPool;
        _explosionPool = explosionPool;

        _size = Mathf.Clamp(size, 0.45f, 1.35f);
        transform.localScale = Vector3.one * _size;

        _pickupDropChance = Mathf.Clamp01(pickupDropChance);

        _rotSpeed = Random.Range(rotateDegPerSecMin, rotateDegPerSecMax) * (Random.value < 0.5f ? -1f : 1f);

        _hp = GetHPForSize(_size);

        _rb.linearVelocity = velocity;
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, _rotSpeed * Time.deltaTime);

        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;

        ScreenBounds2D.GetWorldBounds(_cam, out float left, out float right, out float bottom, out float top);

        // despawn cuando sale por abajo
        if (transform.position.y < bottom - 2.0f)
        {
            _poolMember.ReturnToPool();
        }
    }

    public void Hit(int damage)
    {
        _hp -= Mathf.Max(1, damage);
        if (_hp <= 0) Die();
    }

    private void Die()
    {
        // Score
        int score = GetScoreForSize(_size);
        if (GameManager.I != null) GameManager.I.AddScore(score);

        // ✅ Explosión (pool desde escena)
        if (_explosionPool != null)
            _explosionPool.Get(transform.position, Quaternion.identity);

        // Pickup chance
        if (_pickupPool != null && Random.value < _pickupDropChance)
            _pickupPool.Get(transform.position, Quaternion.identity);

        // Split controlado
        SplitIfNeeded();

        // Return to pool
        _poolMember.ReturnToPool();
    }

    private void SplitIfNeeded()
    {
        if (_spawner == null) return;

        // grande -> 2 medianos
        if (_size >= 1.0f)
        {
            SpawnChild(0.70f, new Vector2(-1.1f, 0.2f));
            SpawnChild(0.70f, new Vector2(1.1f, 0.2f));
            return;
        }

        // mediano -> 2 pequeños
        if (_size >= 0.70f)
        {
            SpawnChild(0.52f, new Vector2(-1.3f, 0.4f));
            SpawnChild(0.52f, new Vector2(1.3f, 0.4f));
        }
    }

    private void SpawnChild(float sizeMul, Vector2 lateralKick)
    {
        float childSize = _size * sizeMul;

        Vector2 baseVel = _rb.linearVelocity;
        Vector2 childVel = baseVel + lateralKick;

        // baja un poco chance de pickup en fragmentos
        float childPickupChance = _pickupDropChance * 0.85f;

        _spawner.SpawnFragment(transform.position, childSize, childVel, childPickupChance);
    }

    private int GetHPForSize(float s)
    {
        if (s >= 1.0f) return hpLarge;
        if (s >= 0.70f) return hpMedium;
        return hpSmall;
    }

    private int GetScoreForSize(float s)
    {
        if (s >= 1.0f) return scoreLarge;
        if (s >= 0.70f) return scoreMedium;
        return scoreSmall;
    }
}
