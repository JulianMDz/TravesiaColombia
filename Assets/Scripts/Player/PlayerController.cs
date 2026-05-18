using System.Collections;
using UnityEngine;
using TravesiaColombia.Core;

namespace TravesiaColombia.Player
{
    /// <summary>
    /// Controlador principal del jugador. Maneja movimiento, salto, daño y estado.
    /// Se comunica con InputManager via eventos y publica al EventBus.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // ── Estados ──────────────────────────────────────────────────────────
        public enum PlayerState { Idle, Running, Jumping, Falling, Hurt, Dead, Flying }

        [Header("Estado")]
        [SerializeField] private PlayerState _currentState = PlayerState.Idle;
        public PlayerState CurrentState => _currentState;

        // ── Referencias ──────────────────────────────────────────────────────
        private Rigidbody _rb;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        // ── Movimiento horizontal ────────────────────────────────────────────
        [Header("Movimiento")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintMultiplier = 1.5f;

        private float _moveInput;
        private bool _isSprinting;

        // ── Salto ────────────────────────────────────────────────────────────
        [Header("Salto")]
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _fallMultiplier = 2.5f;
        [SerializeField] private float _lowJumpMultiplier = 2f;
        [SerializeField] private bool _allowDoubleJump = true;

        private bool _jumpRequested;
        private bool _hasDoubleJump;

        // ── Ground Check ─────────────────────────────────────────────────────
        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayer;

        public bool IsGrounded { get; private set; }
        private bool _wasGrounded;

        // ── Coyote Time & Jump Buffer ────────────────────────────────────────
        [Header("Coyote Time & Buffer")]
        [SerializeField] private float _coyoteTime = 0.15f;
        [SerializeField] private float _jumpBufferTime = 0.1f;

        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;

        // ── Vida y daño ──────────────────────────────────────────────────────
        [Header("Vida")]
        [SerializeField] private int _currentLives = 3;
        [SerializeField] private float _invincibilityTime = 1.5f;
        [SerializeField] private float _knockbackForce = 5f;

        private bool _isInvincible;
        private Coroutine _invincibilityCoroutine;

        // ── Power-Up Vuelo ───────────────────────────────────────────────────
        private bool _isFlying;
        private float _flyingTimeLeft;

        // ── Visual ───────────────────────────────────────────────────────────
        private bool _facingRight = true;

        // ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();

            if (_groundCheck == null)
            {
                GameObject gc = new GameObject("GroundCheck");
                gc.transform.SetParent(transform);
                gc.transform.localPosition = new Vector3(0, -0.5f, 0);
                _groundCheck = gc.transform;
            }
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMove += HandleMove;
                InputManager.Instance.OnJump += HandleJump;
                InputManager.Instance.OnSprint += HandleSprint;
            }

            EventBus.Subscribe<PowerUpActivated>(OnPowerUpActivated);
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMove -= HandleMove;
                InputManager.Instance.OnJump -= HandleJump;
                InputManager.Instance.OnSprint -= HandleSprint;
            }

            EventBus.Unsubscribe<PowerUpActivated>(OnPowerUpActivated);
        }

        private void Update()
        {
            CheckGroundStatus();
            UpdateCoyoteTime();
            UpdateJumpBuffer();
            UpdateState();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyGravityModifiers();
            ProcessJump();
        }

        // ── Input Handlers ───────────────────────────────────────────────────

        private void HandleMove(Vector2 input)
        {
            if (IsBlocked()) return;
            _moveInput = input.x;
        }

        private void HandleJump()
        {
            if (IsBlocked()) return;
            _jumpRequested = true;
            _jumpBufferCounter = _jumpBufferTime;
        }

        private void HandleSprint()
        {
            if (IsBlocked()) return;
            _isSprinting = !_isSprinting;
        }

        // ── Movement ─────────────────────────────────────────────────────────

        private void ApplyMovement()
        {
            if (IsBlocked()) return;

            float speed = _moveSpeed;
            if (_isSprinting && IsGrounded) speed *= _sprintMultiplier;

            Vector3 targetVelocity = new Vector3(_moveInput * speed, _rb.linearVelocity.y, 0);
            _rb.linearVelocity = targetVelocity;

            if (_moveInput > 0 && !_facingRight) Flip();
            else if (_moveInput < 0 && _facingRight) Flip();
        }

        private void Flip()
        {
            _facingRight = !_facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        // ── Jump ─────────────────────────────────────────────────────────────

        private void ProcessJump()
        {
            if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0)
            {
                PerformJump();
                _jumpBufferCounter = 0;
            }
            else if (_jumpRequested && _allowDoubleJump && _hasDoubleJump && !IsGrounded)
            {
                PerformJump();
                _hasDoubleJump = false;
            }

            _jumpRequested = false;
        }

        private void PerformJump()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpForce, 0);
            _coyoteTimeCounter = 0;
            EventBus.Publish(new PlayerJumped());
        }

        private void ApplyGravityModifiers()
        {
            if (_rb.linearVelocity.y < 0)
            {
                _rb.linearVelocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (_rb.linearVelocity.y > 0 && !InputManager.Instance.IsJumping)
            {
                _rb.linearVelocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        // ── Ground Check ─────────────────────────────────────────────────────

        private void CheckGroundStatus()
        {
            _wasGrounded = IsGrounded;
            IsGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer);

            if (IsGrounded && !_wasGrounded)
            {
                EventBus.Publish(new PlayerLanded());
                _hasDoubleJump = true;
            }
        }

        private void UpdateCoyoteTime()
        {
            if (IsGrounded)
                _coyoteTimeCounter = _coyoteTime;
            else
                _coyoteTimeCounter -= Time.deltaTime;
        }

        private void UpdateJumpBuffer()
        {
            if (_jumpBufferCounter > 0)
                _jumpBufferCounter -= Time.deltaTime;
        }

        // ── State Machine ────────────────────────────────────────────────────

        private void UpdateState()
        {
            if (_currentState == PlayerState.Dead) return;
            if (_currentState == PlayerState.Hurt) return;

            if (_isFlying)
                _currentState = PlayerState.Flying;
            else if (!IsGrounded && _rb.linearVelocity.y > 0.1f)
                _currentState = PlayerState.Jumping;
            else if (!IsGrounded && _rb.linearVelocity.y < -0.1f)
                _currentState = PlayerState.Falling;
            else if (Mathf.Abs(_moveInput) > 0.01f)
                _currentState = PlayerState.Running;
            else
                _currentState = PlayerState.Idle;
        }

        private bool IsBlocked()
        {
            return _currentState == PlayerState.Dead ||
                   _currentState == PlayerState.Hurt ||
                   !InputManager.Instance.GameplayEnabled;
        }

        // ── Damage ───────────────────────────────────────────────────────────

        public void TakeDamage(int damage, Vector3 hitDirection)
        {
            if (_isInvincible || _currentState == PlayerState.Dead) return;

            _currentLives -= damage;
            EventBus.Publish(new PlayerHurt(_currentLives));

            if (_currentLives <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(HurtState(hitDirection));
            }
        }

        private IEnumerator HurtState(Vector3 hitDirection)
        {
            _currentState = PlayerState.Hurt;

            // Knockback
            Vector3 knockback = new Vector3(hitDirection.x * _knockbackForce, _knockbackForce * 0.5f, 0);
            _rb.linearVelocity = knockback;

            // Invincibilidad
            if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = StartCoroutine(InvincibilityFlash());

            yield return new WaitForSeconds(0.3f);

            if (_currentState == PlayerState.Hurt)
                _currentState = PlayerState.Idle;
        }

        private IEnumerator InvincibilityFlash()
        {
            _isInvincible = true;
            float elapsed = 0f;

            while (elapsed < _invincibilityTime)
            {
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            _spriteRenderer.enabled = true;
            _isInvincible = false;
        }

        private void Die()
        {
            _currentState = PlayerState.Dead;
            _rb.linearVelocity = Vector3.zero;
            EventBus.Publish(new PlayerDied());
        }

        // ── Power-Ups ────────────────────────────────────────────────────────

        private void OnPowerUpActivated(PowerUpActivated e)
        {
            if (e.type == PowerUpType.Vuelo)
            {
                _isFlying = true;
                _flyingTimeLeft = e.duration;
                StartCoroutine(FlyingMode(e.duration));
            }
        }

        private IEnumerator FlyingMode(float duration)
        {
            yield return new WaitForSeconds(duration);
            _isFlying = false;
        }

        // ── Animator ─────────────────────────────────────────────────────────

        private void UpdateAnimator()
        {
            if (_animator == null) return;

            _animator.SetFloat("Speed", Mathf.Abs(_moveInput));
            _animator.SetBool("IsGrounded", IsGrounded);
            _animator.SetBool("IsJumping", _currentState == PlayerState.Jumping);
            _animator.SetBool("IsFalling", _currentState == PlayerState.Falling);
            _animator.SetBool("IsHurt", _currentState == PlayerState.Hurt);
            _animator.SetBool("IsDead", _currentState == PlayerState.Dead);
            _animator.SetBool("IsFlying", _isFlying);
        }

        // ── Collisions ───────────────────────────────────────────────────────

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Hazard"))
            {
                Vector3 hitDir = (transform.position - other.transform.position).normalized;
                TakeDamage(1, hitDir);
            }
        }

        // ── Gizmos ───────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            if (_groundCheck != null)
            {
                Gizmos.color = IsGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
            }
        }
    }
}