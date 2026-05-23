using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TravesiaColombia.Core;

namespace TravesiaColombia.Player
{
    /// <summary>
    /// Gestiona todas las entradas del jugador usando New Input System.
    /// Publica eventos al EventBus y expone propiedades de estado.
    /// No contiene logica de juego — solo captura y distribuye inputs.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────
        public static InputManager Instance { get; private set; }

        // ── Input Actions ────────────────────────────────────────────────────
        [Header("Input Action Asset")]
        [SerializeField] private InputActionAsset _inputActions;

        // Action Maps
        private InputActionMap _gameplayMap;
        private InputActionMap _uiMap;

        // Acciones individuales
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _interactAction;
        private InputAction _pauseAction;

        // ── Estado actual ────────────────────────────────────────────────────
        public Vector2 MoveInput { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool GameplayEnabled { get; private set; }

        // ── Eventos ──────────────────────────────────────────────────────────
        public event Action<Vector2> OnMove;
        public event Action OnJump;
        public event Action OnSprint;
        public event Action OnInteract;
        public event Action OnPause;

        // ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeActions();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GamePaused>(OnGamePaused);
            EventBus.Subscribe<GameResumed>(OnGameResumed);
            EventBus.Subscribe<GameOver>(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GamePaused>(OnGamePaused);
            EventBus.Unsubscribe<GameResumed>(OnGameResumed);
            EventBus.Unsubscribe<GameOver>(OnGameOver);
            DisableGameplayInput();
        }

        // ── Inicializacion ───────────────────────────────────────────────────

        private void InitializeActions()
        {
            if (_inputActions == null)
            {
                Debug.LogError("[InputManager] InputActionAsset no asignado en el Inspector.");
                return;
            }

            _gameplayMap = _inputActions.FindActionMap("Gameplay", true);
            //_uiMap = _inputActions.FindActionMap("UI", true);

            _moveAction = _gameplayMap.FindAction("Move", true);
            _jumpAction = _gameplayMap.FindAction("Jump", true);
            _sprintAction = _gameplayMap.FindAction("Sprint", true);
            _interactAction = _gameplayMap.FindAction("Interact", true);
            _pauseAction = _gameplayMap.FindAction("Pause", true);

            BindActions();
            EnableGameplayInput();
        }

        private void BindActions()
        {
            _moveAction.performed += ctx => { MoveInput = ctx.ReadValue<Vector2>(); OnMove?.Invoke(MoveInput); };
            _moveAction.canceled += ctx => { MoveInput = Vector2.zero; OnMove?.Invoke(MoveInput); };

            _jumpAction.performed += ctx => { IsJumping = true; OnJump?.Invoke(); };
            _jumpAction.canceled += ctx => { IsJumping = false; };

            _sprintAction.performed += ctx => { IsSprinting = true; OnSprint?.Invoke(); };
            _sprintAction.canceled += ctx => { IsSprinting = false; };

            _interactAction.performed += ctx => OnInteract?.Invoke();
            _pauseAction.performed += ctx => OnPause?.Invoke();
        }

        // ── Habilitar / deshabilitar ─────────────────────────────────────────

        /// <summary>Activa el ActionMap de gameplay.</summary>
        public void EnableGameplayInput()
        {
            _gameplayMap?.Enable();
            GameplayEnabled = true;
        }

        /// <summary>Desactiva el ActionMap de gameplay (durante menus o cinematicas).</summary>
        public void DisableGameplayInput()
        {
            _gameplayMap?.Disable();
            GameplayEnabled = false;
            MoveInput = Vector2.zero;
            IsJumping = false;
            IsSprinting = false;
        }

        // ── Callbacks EventBus ───────────────────────────────────────────────

        private void OnGamePaused(GamePaused e) => DisableGameplayInput();
        private void OnGameResumed(GameResumed e) => EnableGameplayInput();
        private void OnGameOver(GameOver e) => DisableGameplayInput();
    }
}