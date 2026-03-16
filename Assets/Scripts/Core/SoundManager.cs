using UnityEngine;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// Gestiona todo el audio del juego — BGM y SFX.
    /// Suscrito al EventBus para reproducir sonidos reactivamente.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────
        public static SoundManager Instance { get; private set; }

        // ── Audio Sources ────────────────────────────────────────────────────
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        // ── BGM por región ───────────────────────────────────────────────────
        [Header("BGM por Región")]
        [SerializeField] private AudioClip _bgmSelva;
        [SerializeField] private AudioClip _bgmCosta;
        [SerializeField] private AudioClip _bgmEjeCafetero;
        [SerializeField] private AudioClip _bgmMainMenu;

        // ── SFX ──────────────────────────────────────────────────────────────
        [Header("SFX — Coleccionables")]
        [SerializeField] private AudioClip _sfxCoin;
        [SerializeField] private AudioClip _sfxCard;
        [SerializeField] private AudioClip _sfxPowerUp;

        [Header("SFX — Jugador")]
        [SerializeField] private AudioClip _sfxPlayerHurt;
        [SerializeField] private AudioClip _sfxPlayerJump;

        [Header("SFX — Enemigos")]
        [SerializeField] private AudioClip _sfxEnemyDefeated;

        [Header("SFX — GameState")]
        [SerializeField] private AudioClip _sfxGameOver;
        [SerializeField] private AudioClip _sfxLevelComplete;
        [SerializeField] private AudioClip _sfxPause;

        // ── Volúmenes ────────────────────────────────────────────────────────
        [Header("Volúmenes (0 a 1)")]
        [SerializeField][Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField][Range(0f, 1f)] private float _musicVolume = 1f;
        [SerializeField][Range(0f, 1f)] private float _sfxVolume = 1f;

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

            // Crear AudioSources si no están asignados en el Inspector
            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.loop = true;
            }
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.loop = false;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<CoinCollected>(OnCoinCollected);
            EventBus.Subscribe<CardFound>(OnCardFound);
            EventBus.Subscribe<PowerUpActivated>(OnPowerUpActivated);
            EventBus.Subscribe<PlayerHurt>(OnPlayerHurt);
            EventBus.Subscribe<PlayerJumped>(OnPlayerJumped);
            EventBus.Subscribe<EnemyDefeated>(OnEnemyDefeated);
            EventBus.Subscribe<GamePaused>(OnGamePaused);
            EventBus.Subscribe<GameOver>(OnGameOver);
            EventBus.Subscribe<LevelCompleted>(OnLevelCompleted);
            EventBus.Subscribe<RegionUnlocked>(OnRegionUnlocked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CoinCollected>(OnCoinCollected);
            EventBus.Unsubscribe<CardFound>(OnCardFound);
            EventBus.Unsubscribe<PowerUpActivated>(OnPowerUpActivated);
            EventBus.Unsubscribe<PlayerHurt>(OnPlayerHurt);
            EventBus.Unsubscribe<PlayerJumped>(OnPlayerJumped);
            EventBus.Unsubscribe<EnemyDefeated>(OnEnemyDefeated);
            EventBus.Unsubscribe<GamePaused>(OnGamePaused);
            EventBus.Unsubscribe<GameOver>(OnGameOver);
            EventBus.Unsubscribe<LevelCompleted>(OnLevelCompleted);
            EventBus.Unsubscribe<RegionUnlocked>(OnRegionUnlocked);
        }

        // ── Callbacks EventBus ───────────────────────────────────────────────

        private void OnCoinCollected(CoinCollected e) => PlaySFX(_sfxCoin);
        private void OnCardFound(CardFound e) => PlaySFX(_sfxCard);
        private void OnPowerUpActivated(PowerUpActivated e) => PlaySFX(_sfxPowerUp);
        private void OnPlayerHurt(PlayerHurt e) => PlaySFX(_sfxPlayerHurt);
        private void OnPlayerJumped(PlayerJumped e) => PlaySFX(_sfxPlayerJump);
        private void OnEnemyDefeated(EnemyDefeated e) => PlaySFX(_sfxEnemyDefeated);
        private void OnGamePaused(GamePaused e) => PlaySFX(_sfxPause);
        private void OnGameOver(GameOver e) => PlaySFX(_sfxGameOver);
        private void OnLevelCompleted(LevelCompleted e) => PlaySFX(_sfxLevelComplete);

        private void OnRegionUnlocked(RegionUnlocked e)
        {
            PlayBGMByRegion(e.region);
        }

        // ── Métodos públicos ─────────────────────────────────────────────────

        /// <summary>Reproduce BGM según la región activa.</summary>
        public void PlayBGMByRegion(RegionType region)
        {
            AudioClip clip = region switch
            {
                RegionType.Selva => _bgmSelva,
                RegionType.Costa => _bgmCosta,
                RegionType.EjeCafetero => _bgmEjeCafetero,
                _ => _bgmMainMenu
            };
            PlayBGM(clip);
        }

        /// <summary>Reproduce un clip de música de fondo en loop.</summary>
        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || _bgmSource == null) return;
            if (_bgmSource.clip == clip) return;
            _bgmSource.clip = clip;
            _bgmSource.volume = _musicVolume * _masterVolume;
            _bgmSource.Play();
        }

        /// <summary>Detiene la música de fondo.</summary>
        public void StopBGM()
        {
            if (_bgmSource != null) _bgmSource.Stop();
        }

        /// <summary>Reproduce un efecto de sonido.</summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.volume = _sfxVolume * _masterVolume;
            _sfxSource.PlayOneShot(clip);
        }

        /// <summary>Ajusta el volumen maestro (0 a 1).</summary>
        public void SetMasterVolume(float value)
        {
            _masterVolume = Mathf.Clamp01(value);
            ApplyVolumes();
        }

        /// <summary>Ajusta el volumen de música (0 a 1).</summary>
        public void SetMusicVolume(float value)
        {
            _musicVolume = Mathf.Clamp01(value);
            ApplyVolumes();
        }

        /// <summary>Ajusta el volumen de efectos (0 a 1).</summary>
        public void SetSFXVolume(float value)
        {
            _sfxVolume = Mathf.Clamp01(value);
        }

        // ── Privados ─────────────────────────────────────────────────────────

        private void ApplyVolumes()
        {
            if (_bgmSource != null)
                _bgmSource.volume = _musicVolume * _masterVolume;
        }
    }
}