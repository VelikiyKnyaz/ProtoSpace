using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float clampPadding = 0.35f;

    [Header("Refs")]
    [SerializeField] private SpriteRenderer shipRenderer;
    [SerializeField] private Collider2D shipCollider;

    public WeaponController Weapon { get; private set; }

    private Rigidbody2D _rb;
    private Camera _cam;

    private bool _controlEnabled = true;
    private bool _invulnerable = false;
    private float _invulnTimer = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        Weapon = GetComponent<WeaponController>();
    }

    private void Update()
    {
        if (_invulnerable)
        {
            _invulnTimer -= Time.deltaTime;
            if (_invulnTimer <= 0f) _invulnerable = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_controlEnabled) { _rb.linearVelocity = Vector2.zero; return; }

        Vector2 move = GetMoveInput();
        _rb.linearVelocity = move * moveSpeed;

        ClampToScreen();
    }

    private Vector2 GetMoveInput()
    {
        // 1) Mouse/touch drag: si mantienes click, la nave “persigue” el puntero
        if (Input.GetMouseButton(0))
        {
            Vector3 mp = Input.mousePosition;
            Vector3 wp = _cam.ScreenToWorldPoint(mp);
            Vector2 dir = ((Vector2)wp - _rb.position);
            return Vector2.ClampMagnitude(dir, 1f);
        }

        // 2) Keyboard fallback
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 v = new Vector2(x, y).normalized;
        return v;
    }

    private void ClampToScreen()
    {
        ScreenBounds2D.GetWorldBounds(_cam, out float left, out float right, out float bottom, out float top);

        Vector2 p = _rb.position;
        p.x = Mathf.Clamp(p.x, left + clampPadding, right - clampPadding);
        p.y = Mathf.Clamp(p.y, bottom + clampPadding, top - clampPadding);

        _rb.position = p;
    }

    public void SetInvulnerable(float seconds)
    {
        _invulnerable = true;
        _invulnTimer = seconds;
    }

    public void SetControlEnabled(bool enabled) => _controlEnabled = enabled;

    public void SetActiveVisual(bool active)
    {
        if (shipRenderer != null) shipRenderer.enabled = active;
        if (shipCollider != null) shipCollider.enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_invulnerable) return;

        if (other.CompareTag("Asteroid"))
        {
            GameManager.I.PlayerHit();
        }
    }
}
