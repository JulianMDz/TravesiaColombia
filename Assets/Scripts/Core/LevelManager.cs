using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// Gestiona regiones, carga de escenas y progresión de niveles.
    /// Singleton persistente entre escenas.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────
        public static LevelManager Instance { get; private set; }

        // ── Región actual ────────────────────────────────────────────────────
        [Header("Región actual")]
        [SerializeField] private RegionType _currentRegion = RegionType.Selva;
        public RegionType CurrentRegion => _currentRegion;

        // ── Regiones desbloqueadas ───────────────────────────────────────────
        [Header("Progresión")]
        [SerializeField] private List<RegionType> _unlockedRegions = new List<RegionType>();
        public IReadOnlyList<RegionType> UnlockedRegions => _unlockedRegions;

        // ── Carga de escena ──────────────────────────────────────────────────
        private bool _isLoading = false;

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

            // Selva desbloqueada por defecto
            if (!_unlockedRegions.Contains(RegionType.Selva))
                _unlockedRegions.Add(RegionType.Selva);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<LevelCompleted>(OnLevelCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelCompleted>(OnLevelCompleted);
        }

        // ── Callbacks EventBus ───────────────────────────────────────────────

        private void OnLevelCompleted(LevelCompleted e)
        {
            UnlockNextRegion(e.region);
        }

        // ── Métodos públicos ─────────────────────────────────────────────────

        /// <summary>Carga una escena de forma asíncrona.</summary>
        public void LoadScene(string sceneName)
        {
            if (_isLoading) return;
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        /// <summary>Marca una región como desbloqueada y publica el evento.</summary>
        public void UnlockRegion(RegionType region)
        {
            if (_unlockedRegions.Contains(region)) return;
            _unlockedRegions.Add(region);
            EventBus.Publish(new RegionUnlocked(region));
        }

        /// <summary>Verifica si una región está desbloqueada.</summary>
        public bool IsRegionUnlocked(RegionType region)
        {
            return _unlockedRegions.Contains(region);
        }

        /// <summary>Notifica que el nivel actual fue completado.</summary>
        public void LevelCompleted(float timeElapsed)
        {
            EventBus.Publish(new LevelCompleted(_currentRegion, timeElapsed));
        }

        // ── Privados ─────────────────────────────────────────────────────────

        private void UnlockNextRegion(RegionType completedRegion)
        {
            switch (completedRegion)
            {
                case RegionType.Selva:
                    UnlockRegion(RegionType.Costa);
                    break;
                case RegionType.Costa:
                    UnlockRegion(RegionType.EjeCafetero);
                    break;
            }
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            _isLoading = true;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
                yield return null;

            _isLoading = false;
        }
    }
}