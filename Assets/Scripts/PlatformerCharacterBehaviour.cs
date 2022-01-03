using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlatformerCharacterBehaviour : MonoBehaviour
{
    public float movementSpeed = 10f;
    [SerializeField]
    private bool _isFacingRight = true;

    [Header("Physics")]
    public float jumpForce = 5f;
    [SerializeField]
    private ContactFilter2D _groundFilter;
    public ContactFilter2D GroundFilter => _groundFilter;
    public float gravity = 1f;
    public float fallModifier = 2.5f;

    [Header("Sub-Components")]
    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private Collider2D _collider2D;
    public Collider2D Collider2D => _collider2D;

    private Vector2 _direction = Vector2.zero;
    private bool _jump = false;
    private bool _isGrounded = false;
    public bool IsGrounded => _isGrounded;

    public void UpdateMovement(Vector2 direction)
    {
        _direction = direction;
    }

    public void Jump()
    {
        _jump = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
        CalculatePhysics();
    }

    private void CheckGround()
    {
        var results = new RaycastHit2D[5];
        var hitCount = _collider2D.Cast(Vector2.down, _groundFilter, results, .05f, true);
        if (hitCount > 0)
        {
            if (!_isGrounded)
            {
                // fire land event
            }
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void CalculatePhysics()
    {
        var velocity = _rigidbody2D.velocity;

        // Movement
        _rigidbody2D.velocity = new Vector2(_direction.x * movementSpeed * Time.fixedDeltaTime, velocity.y);
        if ((_direction.x > 0f && !_isFacingRight) || (_direction.x < 0f && _isFacingRight))
        {
            Flip();
        }

        // Jump
        if (_jump)
        {
            if (_isGrounded)
            {
                _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            _jump = false;
        }

        // Gravity
        if (velocity.y < 0f && !_isGrounded)
        {
            _rigidbody2D.gravityScale = gravity * fallModifier;
        }
        else
        {
            _rigidbody2D.gravityScale = gravity;
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        _transform.Rotate(0f, 180f, 0f);
    }

    private void Reset()
    {
        _transform = transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }
}
