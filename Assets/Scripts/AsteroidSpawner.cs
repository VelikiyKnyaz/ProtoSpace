using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField] private GameObjectPool asteroidPool;
    [SerializeField] private GameObjectPool pickupPool;
    [SerializeField] private GameObjectPool explosionPool;

    [Header("Spawn")]
    [SerializeField] private float spawnPaddingTop = 1.2f;
    [SerializeField] private float safeRadiusFromPlayer = 2.0f;

    [Header("Refs")]
    [SerializeField] private Transform playerTransform;

    private Camera _cam;
    private float _spawnTimer;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (GameManager.I == null || GameManager.I.IsGameOver) return;
        if (DifficultyDirector.I == null) return;

        // Si no hay cámara main, no spawnea (evita null)
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;

        _spawnTimer -= Time.deltaTime;

        int max = DifficultyDirector.I.MaxAsteroids();
        int current = Asteroid.ActiveCount;

        if (_spawnTimer <= 0f && current < max)
        {
            SpawnAsteroid();
            _spawnTimer = DifficultyDirector.I.SpawnInterval();
        }
    }

    private void SpawnAsteroid()
    {
        if (asteroidPool == null) return;

        ScreenBounds2D.GetWorldBounds(_cam, out float left, out float right, out float bottom, out float top);

        float x = Random.Range(left, right);
        float y = top + spawnPaddingTop;

        Vector2 pos = new Vector2(x, y);

        if (playerTransform != null && Vector2.Distance(pos, playerTransform.position) < safeRadiusFromPlayer)
        {
            // Re-roll simple para evitar que caiga encima del player
            x = (x < 0) ? right : left;
            pos = new Vector2(x, y);
        }

        var member = asteroidPool.Get(pos, Quaternion.identity);
        if (member == null) return;

        var asteroid = member.GetComponent<Asteroid>();
        if (asteroid == null) return;

        float difficulty = DifficultyDirector.I.Difficulty01;
        float baseSpeed = DifficultyDirector.I.AsteroidSpeed();

        bool large = Random.value < DifficultyDirector.I.LargeChance();
        float size = large ? Random.Range(1.0f, 1.25f) : Random.Range(0.55f, 0.95f);

        float drift = Mathf.Lerp(0.4f, 1.6f, difficulty) * Random.Range(-1f, 1f);
        Vector2 vel = new Vector2(drift, -baseSpeed * Random.Range(0.85f, 1.15f));

        float pickupDropChance = DifficultyDirector.I.PickupDropChance();

        // ✅ Inyecta pickupPool y explosionPool desde escena
        asteroid.Init(this, pickupPool, explosionPool, size, vel, pickupDropChance);
    }

    // Llamado desde Asteroid para fragmentos
    public void SpawnFragment(Vector2 position, float size, Vector2 velocity, float pickupDropChance)
    {
        if (asteroidPool == null) return;

        var member = asteroidPool.Get(position, Quaternion.identity);
        if (member == null) return;

        var asteroid = member.GetComponent<Asteroid>();
        if (asteroid == null) return;

        // ✅ Inyecta pickupPool y explosionPool también a los fragmentos
        asteroid.Init(this, pickupPool, explosionPool, size, velocity, pickupDropChance);
    }
}
