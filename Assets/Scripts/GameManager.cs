using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Core")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Rules")]
    [SerializeField] private int startLives = 3;
    [SerializeField] private float respawnDelay = 1.0f;
    [SerializeField] private float respawnInvuln = 1.5f;

    public int Score { get; private set; }
    public int Lives { get; private set; }
    public bool IsGameOver { get; private set; }

    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;

    public float ElapsedTime { get; private set; }

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    private void Start()
    {
        Lives = startLives;
        Score = 0;
        IsGameOver = false;

        OnLivesChanged?.Invoke(Lives);
        OnScoreChanged?.Invoke(Score);

        if (playerSpawnPoint != null && player != null)
            player.transform.position = playerSpawnPoint.position;
    }

    private void Update()
    {
        if (IsGameOver) return;
        ElapsedTime += Time.deltaTime;

        // “Score de supervivencia” suave para partidas largas:
        // 1 punto cada 2 segundos (no rompe el balance).
        if (Time.frameCount % 120 == 0) AddScore(1);
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score = Mathf.Max(0, Score + amount);
        OnScoreChanged?.Invoke(Score);
    }

    public void PlayerHit()
    {
        if (IsGameOver) return;

        Lives--;
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
        {
            IsGameOver = true;
            OnGameOver?.Invoke();
            return;
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // pausa el arma para que no consuma timer ni dispare mientras está “muerto”
        player.SetActiveVisual(false);
        player.SetControlEnabled(false);
        player.Weapon.SetPaused(true);

        yield return new WaitForSeconds(respawnDelay);

        if (playerSpawnPoint != null) player.transform.position = playerSpawnPoint.position;

        player.SetActiveVisual(true);
        player.SetControlEnabled(true);
        player.SetInvulnerable(respawnInvuln);
        player.Weapon.SetPaused(false);
    }
}
