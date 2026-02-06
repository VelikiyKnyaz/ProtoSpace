using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PickupPowerup : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2.5f;
    [SerializeField] private float rotateSpeed = 120f;

    private Rigidbody2D _rb;
    private PoolMember _poolMember;
    private Camera _cam;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _poolMember = GetComponent<PoolMember>();
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        _rb.linearVelocity = Vector2.down * fallSpeed;
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

        ScreenBounds2D.GetWorldBounds(_cam, out float left, out float right, out float bottom, out float top);
        if (transform.position.y < bottom - 2.0f)
            _poolMember.ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var weapon = other.GetComponent<WeaponController>();
        if (weapon != null)
        {
            weapon.AddPowerup();
            _poolMember.ReturnToPool();
        }
    }
}
