using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHud : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Lives")]
    [SerializeField] private Image[] lifeIcons; // tus 3 imágenes

    [Header("Powerup Bar")]
    [SerializeField] private GameObject powerupRoot; // opcional (para ocultar)
    [SerializeField] private Image powerupFill;       // Image Filled vertical

    [Header("Refs")]
    [SerializeField] private WeaponController playerWeapon;

    private void Start()
    {
        if (GameManager.I != null)
        {
            GameManager.I.OnScoreChanged += HandleScore;
            GameManager.I.OnLivesChanged += HandleLives;

            HandleScore(GameManager.I.Score);
            HandleLives(GameManager.I.Lives);
        }

        if (playerWeapon != null)
        {
            playerWeapon.OnPowerupTimerChanged += HandlePowerupTimer;
            HandlePowerupTimer(playerWeapon.PowerupRemaining, 14f);
        }
    }

    private void HandleScore(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    private void HandleLives(int lives)
    {
        if (lifeIcons == null) return;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].enabled = (i < lives);
        }
    }

    private void HandlePowerupTimer(float remaining, float max)
    {
        if (powerupFill != null)
        {
            float f = (max <= 0.01f) ? 0f : Mathf.Clamp01(remaining / max);
            powerupFill.fillAmount = f;
        }

        if (powerupRoot != null)
            powerupRoot.SetActive(remaining > 0.01f);
    }
}
