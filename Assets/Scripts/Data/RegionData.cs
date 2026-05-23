using System.Collections.Generic;
using UnityEngine;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// ScriptableObject que define los datos de una región del juego.
    /// Crear desde: Assets → Create → TravesiaColombia → RegionData
    /// </summary>
    [CreateAssetMenu(fileName = "NewRegion",
                     menuName = "TravesiaColombia/RegionData")]
    public class RegionData : ScriptableObject
    {
        [Header("Identificación")]
        [SerializeField] private RegionType _regionType;
        [SerializeField] private string _regionName;
        [SerializeField][TextArea(2, 3)] private string _description;

        [Header("Visual")]
        [SerializeField] private Sprite _mapIcon;
        [SerializeField] private Color _regionColor = Color.white;

        [Header("Escena")]
        [SerializeField] private string _sceneName;

        [Header("Audio")]
        [SerializeField] private AudioClip _bgmClip;

        [Header("Moneda de la región")]
        [SerializeField] private string _coinName;
        [SerializeField] private Sprite _coinSprite;

        [Header("Cartas disponibles")]
        [SerializeField]
        private List<CardData> _availableCards
            = new List<CardData>();

        [Header("Power-ups disponibles")]
        [SerializeField]
        private List<ShopItem> _availablePowerUps
            = new List<ShopItem>();

        [Header("Progresión")]
        [SerializeField] private bool _isUnlockedByDefault = false;

        // ── Propiedades públicas ──────────────────────────────────────────────
        public RegionType RegionType => _regionType;
        public string RegionName => _regionName;
        public string Description => _description;
        public Sprite MapIcon => _mapIcon;
        public Color RegionColor => _regionColor;
        public string SceneName => _sceneName;
        public AudioClip BGMClip => _bgmClip;
        public string CoinName => _coinName;
        public Sprite CoinSprite => _coinSprite;
        public IReadOnlyList<CardData> AvailableCards => _availableCards;
        public IReadOnlyList<ShopItem> AvailablePowerUps => _availablePowerUps;
        public bool IsUnlockedByDefault => _isUnlockedByDefault;
    }
}