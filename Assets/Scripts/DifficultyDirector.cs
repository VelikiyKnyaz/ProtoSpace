using UnityEngine;

public class DifficultyDirector : MonoBehaviour
{
    public static DifficultyDirector I { get; private set; }

    [Header("Difficulty Timeline")]
    [Tooltip("Cuántos minutos hasta llegar a la dificultad máxima (aprox).")]
    [SerializeField] private float minutesToMax = 8f;

    [Header("Spawn Interval (seconds)")]
    [SerializeField] private float spawnIntervalEasy = 1.25f;
    [SerializeField] private float spawnIntervalHard = 0.35f;

    [Header("Asteroid Speed")]
    [SerializeField] private float asteroidSpeedEasy = 1.3f;
    [SerializeField] private float asteroidSpeedHard = 4.8f;

    [Header("Max Concurrent Asteroids")]
    [SerializeField] private int maxAsteroidsEasy = 6;
    [SerializeField] private int maxAsteroidsHard = 20;

    [Header("Large Spawn Chance")]
    [SerializeField] private float largeChanceEasy = 0.75f;
    [SerializeField] private float largeChanceHard = 0.40f;

    [Header("Pickup Drop Chance (on asteroid death)")]
    [SerializeField] private float pickupDropEasy = 0.13f;
    [SerializeField] private float pickupDropHard = 0.08f;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public float Difficulty01
    {
        get
        {
            if (GameManager.I == null) return 0f;
            float t = GameManager.I.ElapsedTime;
            float tMax = Mathf.Max(30f, minutesToMax * 60f);
            float x = Mathf.Clamp01(t / tMax);

            // curva suave: sube lento al principio, luego aprieta, y se estabiliza
            // (evita “pared” de dificultad)
            return SmoothStep01(x);
        }
    }

    public float SpawnInterval() => Mathf.Lerp(spawnIntervalEasy, spawnIntervalHard, Difficulty01);
    public float AsteroidSpeed() => Mathf.Lerp(asteroidSpeedEasy, asteroidSpeedHard, Mathf.Pow(Difficulty01, 0.85f));
    public int MaxAsteroids() => Mathf.RoundToInt(Mathf.Lerp(maxAsteroidsEasy, maxAsteroidsHard, Difficulty01));
    public float LargeChance() => Mathf.Lerp(largeChanceEasy, largeChanceHard, Difficulty01);
    public float PickupDropChance() => Mathf.Lerp(pickupDropEasy, pickupDropHard, Difficulty01);

    private float SmoothStep01(float x) => x * x * (3f - 2f * x);
}
