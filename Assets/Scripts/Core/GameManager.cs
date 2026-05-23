using UnityEngine;

using System.Collections;
using UnityEngine;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// Orquestador global del juego. Singleton persistente entre escenas.
    /// Publica y escucha eventos via EventBus. Nunca llama directamente 
    /// a otros managers.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ── Estado ───────────────────────────────────────────────────────────
        [Header("Estado del juego")]
        [SerializeField] private GameState _currentState = GameState.MainMenu;
        public GameState CurrentState => _currentState;

        // ── Score ────────────────────────────────────────────────────────────
        [Header("Puntuación")]
        [SerializeField] private int _score = 0;
        public int Score => _score;

        // ── Vidas ────────────────────────────────────────────────────────────
        [Header("Vidas")]
        [SerializeField] private int _lives = 3;
        [SerializeField] private int _maxLives = 3;
        public int Lives => _lives;

        // ── Timer ────────────────────────────────────────────────────────────
        [Header("Timer")]
        [SerializeField] private float _timer = 0f;
        [SerializeField] private float _timerTickInterval = 1f;
        public float Timer => _timer;

        private bool _timerRunning = false;
        private Coroutine _timerCoroutine;

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
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDied>(OnPlayerDied);
            EventBus.Subscribe<LevelCompleted>(OnLevelCompleted);
            EventBus.Subscribe<EnemyDefeated>(OnEnemyDefeated);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDied>(OnPlayerDied);
            EventBus.Unsubscribe<LevelCompleted>(OnLevelCompleted);
            EventBus.Unsubscribe<EnemyDefeated>(OnEnemyDefeated);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                EventBus.Clear();
        }

        // ── Callbacks EventBus ───────────────────────────────────────────────

        private void OnPlayerDied(PlayerDied e)
        {
            _lives--;
            if (_lives <= 0)
                TriggerGameOver();
            else
                EventBus.Publish(new PlayerHurt(_lives));
        }

        private void OnLevelCompleted(LevelCompleted e)
        {
            StopTimer();
            _currentState = GameState.LevelComplete;
        }

        private void OnEnemyDefeated(EnemyDefeated e)
        {
            AddScore(e.scoreValue);
        }

        // ── Métodos públicos ─────────────────────────────────────────────────

        /// <summary>Inicia una nueva partida reseteando estado.</summary>
        public void StartGame()
        {
            ResetLevel();
            _currentState = GameState.Playing;
            StartTimer();
        }

        /// <summary>Pausa el juego deteniendo tiempo e inputs.</summary>
        public void PauseGame()
        {
            if (_currentState != GameState.Playing) return;
            _currentState = GameState.Paused;
            Time.timeScale = 0f;
            StopTimer();
            EventBus.Publish(new GamePaused());
        }

        /// <summary>Reanuda el juego desde pausa.</summary>
        public void ResumeGame()
        {
            if (_currentState != GameState.Paused) return;
            _currentState = GameState.Playing;
            Time.timeScale = 1f;
            StartTimer();
            EventBus.Publish(new GameResumed());
        }

        /// <summary>Reinicia score, timer y vidas al valor inicial.</summary>
        public void ResetLevel()
        {
            _score = 0;
            _timer = 0f;
            _lives = _maxLives;
            Time.timeScale = 1f;
        }

        /// <summary>Suma puntos y publica ScoreChanged.</summary>
        public void AddScore(int points)
        {
            int delta = points;
            _score += delta;
            EventBus.Publish(new ScoreChanged(_score, delta));
        }

        // ── GameOver ─────────────────────────────────────────────────────────

        private void TriggerGameOver()
        {
            _currentState = GameState.GameOver;
            StopTimer();
            Time.timeScale = 0f;
            EventBus.Publish(new GameOver());
        }

        // ── Timer ────────────────────────────────────────────────────────────

        private void StartTimer()
        {
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            _timerRunning = true;
            _timerCoroutine = StartCoroutine(TimerRoutine());
        }

        private void StopTimer()
        {
            _timerRunning = false;
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        private IEnumerator TimerRoutine()
        {
            while (_timerRunning)
            {
                yield return new WaitForSeconds(_timerTickInterval);
                _timer += _timerTickInterval;
                EventBus.Publish(new TimerTick(_timer));
            }
        }
    }
}
