using System.Collections;
using UnityEngine;
using TravesiaColombia.Core;

namespace TravesiaColombia.Player
{
    /// <summary>
    /// Controlador de jugador optimizado — solo Idle, Run, Jump, Flying
    ///
    /// Parámetros de Animator:
    ///   isRunning  (Bool)   — movimiento horizontal en suelo
    ///   isJumping  (Bool)   — saltando/cayendo en el aire
    ///   isFlying   (Bool)   — hongo activo (Flappy Bird)
    ///
    /// Estados ignorados por ahora (Hurt, Dead, Falling → se manejan internamente):
    ///   - Hurt:    parpadeo visual pero sin animación de dolor
    ///   - Dead:    parado sin animación específica
    ///   - Falling: se representa con isJumping=true cuando cae
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // ── Estados internos (sin animaciones separadas) ──────────────────────
        public enum PlayerState { Idle, Running, Jumping, Falling, Hurt, Dead, Flying }

        [Header("Estado actual")]
        [SerializeField] private PlayerState _currentState = PlayerState.Idle;
        public PlayerState CurrentState => _currentState;

        // ── Referencias ──────────────────────────────────────────────────────
        private Rigidbody _rb;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        // ── Movimiento horizontal ────────────────────────────────────────────
        [Header("Movimiento")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintMultiplier = 1.5f;

        private float _moveInput;
        private bool _isSprinting;

        // ── Salto ────────────────────────────────────────────────────────────
        [Header("Salto")]
        [SerializeField] private float _jumpForce = 10f;
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
        [Header("Coyote Time y Jump Buffer")]
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

        // ── Power-Up Vuelo (Flappy Bird) ─────────────────────────────────────
        [Header("Vuelo — Flappy Bird")]
        [SerializeField] private float _flyDuration = 8f;
        [SerializeField] private float _flapForce = 7f;
        [SerializeField] private float _flyGravityMult = 1.5f;

        private bool _isFlying;
        private float _flyTimeLeft;
        private bool _flapRequested;

        // ── Visual ───────────────────────────────────────────────────────────
        private bool _facingRight = true;

        // ── Hashes de parámetros Animator ─────────────────────────────────────
        private static readonly int AnimIsRunning = Animator.StringToHash("isRunning");
        private static readonly int AnimIsJumping = Animator.StringToHash("isJumping");
        private static readonly int AnimIsFlying = Animator.StringToHash("isFlying");

        // ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
    
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            if (_rb == null)
            {
                Debug.LogError("[PlayerController] Falta Rigidbody");
                return;
            }

            _rb.constraints = RigidbodyConstraints.FreezePositionZ
                            | RigidbodyConstraints.FreezeRotationX
                            | RigidbodyConstraints.FreezeRotationY;

            if (_groundCheck == null)
            {
                GameObject gc = new GameObject("GroundCheck");
                gc.transform.SetParent(transform);
                gc.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                _groundCheck = gc.transform;
            }

            if (_groundLayer.value == 0)
                _groundLayer = LayerMask.GetMask("Ground");
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
            UpdateFlyTimer();
            UpdateState();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            if (_isFlying)
                ApplyFlyingPhysics();
            else
                ApplyNormalPhysics();
        }

        // ── Input ────────────────────────────────────────────────────────────

        private void HandleMove(Vector2 input)
        {
            if (IsBlocked()) return;
            _moveInput = input.x;
        }

        private void HandleJump()
        {
            if (_currentState == PlayerState.Dead) return;

            if (_isFlying)
            {
                _flapRequested = true;
                return;
            }

            if (IsBlocked()) return;
            _jumpRequested = true;
            _jumpBufferCounter = _jumpBufferTime;
        }

        private void HandleSprint()
        {
            if (IsBlocked()) return;
            _isSprinting = !_isSprinting;
        }

        // ── Física normal ────────────────────────────────────────────────────

        private void ApplyNormalPhysics()
        {
            ApplyMovement();
            ApplyGravityModifiers();
            ProcessJump();
        }

        private void ApplyMovement()
        {
            if (IsBlocked() || _rb == null) return;

            float speed = _isSprinting && IsGrounded
                ? _moveSpeed * _sprintMultiplier
                : _moveSpeed;

            _rb.linearVelocity = new Vector3(_moveInput * speed, _rb.linearVelocity.y, 0f);

            if (_moveInput > 0.01f && !_facingRight) Flip();
            else if (_moveInput < -0.01f && _facingRight) Flip();
        }

        private void ApplyGravityModifiers()
        {
            if (_rb == null) return;

            if (_rb.linearVelocity.y < 0f)
            {
                _rb.linearVelocity += Vector3.up * Physics.gravity.y
                    * (_fallMultiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (_rb.linearVelocity.y > 0f
                  && InputManager.Instance != null
                  && !InputManager.Instance.IsJumping)
            {
                _rb.linearVelocity += Vector3.up * Physics.gravity.y
                    * (_lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
            }
        }

        private void ProcessJump()
        {
            if (_rb == null) return;

            if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
            {
                ExecuteJump();
                _jumpBufferCounter = 0f;
            }
            else if (_jumpRequested && _allowDoubleJump && _hasDoubleJump && !IsGrounded)
            {
                ExecuteJump();
                _hasDoubleJump = false;
            }

            _jumpRequested = false;
        }

        private void ExecuteJump()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpForce, 0f);
            _coyoteTimeCounter = 0f;
            EventBus.Publish(new PlayerJumped());
        }

        private void Flip()
        {
            _facingRight = !_facingRight;
            // Modelo 3D necesita rotación en Y, no escala negativa
            transform.rotation = Quaternion.Euler(
                0f,
                _facingRight ? 90f : -90f,
                0f
            );
        }

        // ── Física de vuelo (Flappy Bird) ────────────────────────────────────

        private void ApplyFlyingPhysics()
        {
            if (_rb == null) return;

            float speed = _moveSpeed * 0.8f;
            _rb.linearVelocity = new Vector3(_moveInput * speed, _rb.linearVelocity.y, 0f);

            if (_moveInput > 0.01f && !_facingRight) Flip();
            else if (_moveInput < -0.01f && _facingRight) Flip();

            if (_flapRequested)
            {
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _flapForce, 0f);
                _flapRequested = false;
            }
            else
            {
                _rb.linearVelocity += Vector3.up * Physics.gravity.y
                    * (_flyGravityMult - 1f) * Time.fixedDeltaTime;
            }
        }

        // ── Ground Check ─────────────────────────────────────────────────────

        private void CheckGroundStatus()
        {
            if (_groundCheck == null) return;

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
            _coyoteTimeCounter = IsGrounded
                ? _coyoteTime
                : _coyoteTimeCounter - Time.deltaTime;
        }

        private void UpdateJumpBuffer()
        {
            if (_jumpBufferCounter > 0f)
                _jumpBufferCounter -= Time.deltaTime;
        }

        private void UpdateFlyTimer()
        {
            if (!_isFlying) return;
            _flyTimeLeft -= Time.deltaTime;
            if (_flyTimeLeft <= 0f)
                _isFlying = false;
        }

        // ── State Machine ────────────────────────────────────────────────────

        private void UpdateState()
        {
            if (_currentState == PlayerState.Dead) return;
            if (_currentState == PlayerState.Hurt) return; // Hurt es temporal, volverá a otro estado

            if (_isFlying)
                _currentState = PlayerState.Flying;
            else if (!IsGrounded && _rb != null && _rb.linearVelocity.y > 0.1f)
                _currentState = PlayerState.Jumping;
            else if (!IsGrounded && _rb != null && _rb.linearVelocity.y < -0.1f)
                _currentState = PlayerState.Falling;
            else if (Mathf.Abs(_moveInput) > 0.01f)
                _currentState = PlayerState.Running;
            else
                _currentState = PlayerState.Idle;
        }

        private bool IsBlocked()
        {
            return _currentState == PlayerState.Dead
                || _currentState == PlayerState.Hurt
                || (InputManager.Instance != null && !InputManager.Instance.GameplayEnabled);
        }

        // ── Damage ───────────────────────────────────────────────────────────

        public void TakeDamage(int damage, Vector3 hitDirection)
        {
            if (_isInvincible || _currentState == PlayerState.Dead) return;

            _currentLives -= damage;
            EventBus.Publish(new PlayerHurt(_currentLives));

            if (_currentLives <= 0)
                Die();
            else
                StartCoroutine(HurtRoutine(hitDirection));
        }

        private IEnumerator HurtRoutine(Vector3 hitDir)
        {
            _currentState = PlayerState.Hurt;

            if (_rb != null)
                _rb.linearVelocity = new Vector3(hitDir.x * _knockbackForce, _knockbackForce * 0.5f, 0f);

            if (_invincibilityCoroutine != null)
                StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = StartCoroutine(InvincibilityFlash());

            yield return new WaitForSeconds(0.35f);

            if (_currentState == PlayerState.Hurt)
                _currentState = PlayerState.Idle;
        }

        private IEnumerator InvincibilityFlash()
        {
            _isInvincible = true;
            float elapsed = 0f;

            while (elapsed < _invincibilityTime)
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.enabled = !_spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = true;
            _isInvincible = false;
        }

        private void Die()
        {
            _currentState = PlayerState.Dead;
            if (_rb != null)
                _rb.linearVelocity = Vector3.zero;
            EventBus.Publish(new PlayerDied());
        }

        // ── Power-Up Vuelo ────────────────────────────────────────────────────

        private void OnPowerUpActivated(PowerUpActivated e)
        {
            if (e.type != PowerUpType.Vuelo) return;

            _isFlying = true;
            _flyTimeLeft = e.duration > 0f ? e.duration : _flyDuration;
            _flapRequested = false;
        }

        // ── Animator — SOLO 3 parámetros ────────────────────────────────────

        private void UpdateAnimator()
        {
            if (_animator == null) return;

            // Solo tres parámetros — TODO lo demás (Falling, Hurt, Dead) se maneja 
            // internamente en la máquina de estados pero NO tiene animaciones separadas

            bool running = _currentState == PlayerState.Running;
            bool jumping = _currentState == PlayerState.Jumping
                        || _currentState == PlayerState.Falling; // Falling también activa Jump anim
            bool flying = _currentState == PlayerState.Flying;

            _animator.SetBool(AnimIsRunning, running);
            _animator.SetBool(AnimIsJumping, jumping);
            _animator.SetBool(AnimIsFlying, flying);
        }

        // ── Colisiones ────────────────────────────────────────────────────────

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
            if (_groundCheck == null) return;
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }
}