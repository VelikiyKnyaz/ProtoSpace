using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Muzzles")]
    [SerializeField] private Transform muzzleCenter;
    [SerializeField] private Transform muzzleLeft;
    [SerializeField] private Transform muzzleRight;

    [Header("Pools")]
    [SerializeField] private GameObjectPool bulletBasePool;
    [SerializeField] private GameObjectPool bulletSpecialPool;

    [Header("Fire")]
    [Tooltip("Disparos por segundo")]
    [SerializeField] private float fireRate = 8.0f;

    [Header("Powerup")]
    [SerializeField] private float durationPerPickup = 8.0f;
    [SerializeField] private float maxPowerupTime = 14.0f;

    public event Action<float, float> OnPowerupTimerChanged;

    public int Tier { get; private set; } = 1; // 1..3
    public float PowerupRemaining { get; private set; } = 0f;

    private float _cooldown = 0f;
    private bool _paused = false;

    public void SetPaused(bool paused) => _paused = paused;

    private void Update()
    {
        if (_paused) return;

        // Timer del powerup
        if (PowerupRemaining > 0f)
        {
            PowerupRemaining -= Time.deltaTime;
            if (PowerupRemaining <= 0f)
            {
                PowerupRemaining = 0f;
                Tier = 1;
            }
            OnPowerupTimerChanged?.Invoke(PowerupRemaining, maxPowerupTime);
        }

        // Manual fire (Space o click)
        bool shoot =
            (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) ||
            (Mouse.current != null && Mouse.current.leftButton.isPressed);

        _cooldown -= Time.deltaTime;

        if (shoot && _cooldown <= 0f)
        {
            Fire();
            _cooldown = 1f / Mathf.Max(0.01f, fireRate);
        }
    }

    public void AddPowerup()
    {
        // Sube tier hasta 3
        if (Tier < 3) Tier++;

        // Extiende tiempo (cap)
        PowerupRemaining = Mathf.Min(maxPowerupTime, PowerupRemaining + durationPerPickup);

        OnPowerupTimerChanged?.Invoke(PowerupRemaining, maxPowerupTime);
    }

    private void Fire()
    {
        if (bulletBasePool == null) return;

        if (Tier == 1)
        {
            SpawnBase(muzzleCenter);
            return;
        }

        if (Tier == 2)
        {
            SpawnBase(muzzleLeft);
            SpawnBase(muzzleRight);
            return;
        }

        // Tier 3: 2 laterales + especial al centro
        SpawnBase(muzzleLeft);
        SpawnBase(muzzleRight);

        if (bulletSpecialPool != null)
            SpawnSpecial(muzzleCenter);
        else
            SpawnBase(muzzleCenter); // fallback si no asignas especial
    }

    private void SpawnBase(Transform muzzle)
    {
        if (muzzle == null) return;
        bulletBasePool.Get(muzzle.position, muzzle.rotation);
    }

    private void SpawnSpecial(Transform muzzle)
    {
        if (muzzle == null) return;
        bulletSpecialPool.Get(muzzle.position, muzzle.rotation);
    }
}
